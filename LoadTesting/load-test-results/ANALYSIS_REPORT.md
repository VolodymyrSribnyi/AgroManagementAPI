# AgroManagement API Load Test Report

**Generated:** 2025-11-30 12:27:20

**API Endpoint:** http://localhost:5122

**Test Duration:** 30 seconds per scenario

## Summary

| Users | Avg (ms) | Min (ms) | Max (ms) | P95 (ms) | Error % | Throughput |
|-------|----------|----------|----------|----------|---------|------------|
| 1 | 29.23 | 2 | 248 | 78.00 | 39.82% | 7.53 req/s |
| 5 | 25.65 | 1 | 520 | 81.00 | 39.85% | 38.73 req/s |
| 20 | 70.71 | 1 | 662 | 210.00 | 39.83% | 114.33 req/s |
| 50 | 298.77 | 3 | 1965 | 839.00 | 39.92% | 123.83 req/s |

## Analysis

**Response Time Scaling:** Linear - Reasonable scalability

## Recommendations

- ⚠️ High error rate detected at higher loads. Consider:
  - Increasing connection pool sizes
  - Adding database query optimization
  - Implementing request rate limiting

