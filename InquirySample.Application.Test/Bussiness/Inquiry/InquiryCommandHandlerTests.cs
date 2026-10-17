using InquirySample.Application.Bussiness.Inquiry.Command;
using InquirySample.Application.Bussiness.Inquiry.Dto;
using InquirySample.Application.Bussiness.Product.Dto;
using InquirySample.Application.Bussiness.Product.Query;
using InquirySample.Application.Common.Interfaces;
using InquirySample.Application.Test.Helper;
using InquirySample.Domain.Enums;
using InquirySample.Infrastruture.Persistance;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace InquirySample.Application.Test.Bussiness.Inquiry
{
    public class InquiryCommandHandlerTests : IDisposable
    {
        private readonly Mock<ISender> _sender = new();
        private readonly Mock<IInquiryExecutionService> _executionService = new();
        private readonly ApplicationDbContext _db = DbContextFactory.Create();
        private readonly InquiryCommandHandler _sut;

        private static readonly ProductDto Product = new(1, "123456");
        private static readonly InquiryCommand Command = new() { HsCode = "123456" };

        public InquiryCommandHandlerTests()
        {
            _sut = new InquiryCommandHandler(_sender.Object, _executionService.Object, _db);
        }

        public void Dispose() => _db.Dispose();

        // ---------- helpers ----------
        private void SetupProduct(ErrorOr<ProductDto> result) =>
            _sender.Setup(x => x.Send(It.IsAny<GetProductByHsCodeQuery>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(result);

        private async Task<Domain.Entities.Inquiry> SetupCreatedInquiry()
        {
            var inquiry = Domain.Entities.Inquiry.Create(Product.Id, DateTime.UtcNow);
            _db.Inquiries.Add(inquiry);
            await _db.SaveChangesAsync();

            _sender.Setup(x => x.Send(It.IsAny<CreateInquiryCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(inquiry);
            return inquiry;
        }

        private void VerifyExecutionNeverCalled() =>
            _executionService.Verify(x => x.ExecuteAsync(
                It.IsAny<InquiryDto>(), It.IsAny<ProductDto>(), It.IsAny<CancellationToken>()),
                Times.Never);

        // ---------- tests ----------
        [Fact]
        public async Task Handle_WhenProductNotFound_ReturnsErrorAndDoesNotCreateInquiry()
        {
            SetupProduct(Error.NotFound());

            var result = await _sut.Handle(Command, CancellationToken.None);

            result.IsError.Should().BeTrue();
            result.FirstError.Type.Should().Be(ErrorType.NotFound);

            _sender.Verify(x => x.Send(It.IsAny<CreateInquiryCommand>(), It.IsAny<CancellationToken>()),
                Times.Never);
            VerifyExecutionNeverCalled();
        }

        [Fact]
        public async Task Handle_WhenCreateInquiryFails_ReturnsErrorAndDoesNotExecute()
        {
            SetupProduct(Product);
            _sender.Setup(x => x.Send(It.IsAny<CreateInquiryCommand>(), It.IsAny<CancellationToken>()))
                   .ReturnsAsync(Error.Failure());

            var result = await _sut.Handle(Command, CancellationToken.None);

            result.IsError.Should().BeTrue();
            VerifyExecutionNeverCalled();
        }

        [Fact]
        public async Task Handle_WhenExecutionFails_MarksInquiryAsFailed()
        {
            SetupProduct(Product);
            var inquiry = await SetupCreatedInquiry();

            _executionService
                .Setup(x => x.ExecuteAsync(It.IsAny<InquiryDto>(), It.IsAny<ProductDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(Error.Failure());

            var result = await _sut.Handle(Command, CancellationToken.None);

            // در این حالت handler خطا برنمی‌گرداند؛ خود inquiry را (با وضعیت Failed) برمی‌گرداند
            result.IsError.Should().BeFalse();

            var saved = await _db.Inquiries.AsNoTracking().FirstAsync(x => x.Id == inquiry.Id);
            saved.InquiryStatus.Should().Be(InquiryStatus.Failed);   // نام property/enum را با دامین خودت تطبیق بده
        }

        [Fact]
        public async Task Handle_WhenExecutionSucceeds_MarksInquiryAsSuccess()
        {
            SetupProduct(Product);
            var inquiry = await SetupCreatedInquiry();

            _executionService
                .Setup(x => x.ExecuteAsync(It.IsAny<InquiryDto>(), It.IsAny<ProductDto>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new InquiryDto(inquiry.Id, 
                                inquiry.ProductId, 
                                Product.HsCode, 
                                1, 
                                DateTime.UtcNow, 
                                DateTime.UtcNow.AddSeconds(1), "OK", InquiryStatus.Success)
                );

            var result = await _sut.Handle(Command, CancellationToken.None);

            result.IsError.Should().BeFalse();

            var saved = await _db.Inquiries.AsNoTracking().FirstAsync(x => x.Id == inquiry.Id);
            saved.InquiryStatus.Should().Be(InquiryStatus.Success);
            saved.ProviderId.Should().Be(1);
        }
    }
}
