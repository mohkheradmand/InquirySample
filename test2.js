import http from 'k6/http';
import { check, sleep } from 'k6';
import { SharedArray } from 'k6/data';

const codes = new SharedArray('hs', () =>
    //open('./hscodesSame.txt').split('\n').map(s => s.trim()).filter(Boolean)
    open('./hscodes.txt').split('\n').map(s => s.trim()).filter(Boolean)
);

// 0 = بدون وقفه (استرس)، عدد بیشتر از 0 = ثانیه‌ی فکر کردن بین درخواست‌ها
const THINK = Number(__ENV.SLEEP || 0);

export const options = {
    insecureSkipTLSVerify: true,
    scenarios: {
        ramp: {
            executor: 'ramping-vus',
            startVUs: 0,
            stages: [
                { duration: '15s', target: 100 },
                { duration: '30s', target: 100 },
                { duration: '15s', target: 300 },
                { duration: '30s', target: 300 },
                { duration: '15s', target: 500 },
                { duration: '30s', target: 500 },
                { duration: '20s', target: 1000 },
                { duration: '30s', target: 1000 },
                { duration: '15s', target: 0 },
            ],
        },
    },
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(95)<1000'],
    },
};

export default function () {
    const code = codes[Math.floor(Math.random() * codes.length)];

    const res = http.post(
        'https://127.0.0.1:7216/api/inquiries',
        JSON.stringify({ hsCode: code }),
        { headers: { 'Content-Type': 'application/json' } }
    );

    const ok = check(res, { 'status is 200': (r) => r.status === 200 });

    if (!ok) {
        console.log(`${code} -> ${res.status} ${res.error} ${String(res.body).slice(0, 200)}`);
    }

    if (THINK > 0) sleep(THINK);
}