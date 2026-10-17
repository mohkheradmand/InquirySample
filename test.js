import http from 'k6/http';
import { check } from 'k6';
import { SharedArray } from 'k6/data';

const codes = new SharedArray('hs', () =>
    //open('./hscodesSame.txt').split('\n').map(s => s.trim()).filter(Boolean)
    open('./hscodes.txt').split('\n').map(s => s.trim()).filter(Boolean)
);

export const options = {
    insecureSkipTLSVerify: true, // برای گواهی self-signed لوکال
    scenarios: {
        ramp: {
            executor: 'ramping-vus',
            startVUs: 0,
            stages: [
                { duration: '15s', target: 100 },  // گرم کردن
                { duration: '30s', target: 100 },  // نگه‌داشتن
                { duration: '15s', target: 300 },
                { duration: '30s', target: 300 },
                { duration: '15s', target: 500 },
                { duration: '30s', target: 500 },
                { duration: '10s', target: 0 },    // پایین آوردن
            ],
        },
    },
    thresholds: {
        http_req_failed: ['rate<0.01'],     // کمتر از ۱٪ خطا
        http_req_duration: ['p(95)<500'],   // ۹۵٪ زیر ۵۰۰ms
    },
};

export default function () {
    const code = codes[Math.floor(Math.random() * codes.length)];

    const res = http.post(
        'https://127.0.0.1:7216/api/inquiries',
        JSON.stringify({ hsCode: code }),
        { headers: { 'Content-Type': 'application/json' } }
    );

    check(res, {
        'status is 200': (r) => r.status === 200,
    });
}