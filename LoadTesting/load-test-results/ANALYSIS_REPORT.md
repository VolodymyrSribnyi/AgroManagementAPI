# AgroManagement API Load Test Report

**Generated:** 2025-12-01 11:20:53

**API Endpoint:** http://localhost:5122

**Test Duration:** 30 seconds per scenario

## Summary

| Users | Avg (ms) | Min (ms) | Max (ms) | P95 (ms) | Error % | Throughput |
|-------|----------|----------|----------|----------|---------|------------|
| 1 | 66.76 | 33 | 357 | 122.00 | 0.00% | 5.80 req/s |
| 5 | 111.95 | 30 | 495 | 260.00 | 0.00% | 22.70 req/s |
| 20 | 475.95 | 45 | 1484 | 853.00 | 0.00% | 34.13 req/s |
| 50 | 1117.93 | 145 | 2704 | 1804.00 | 0.00% | 40.27 req/s |

## Analysis

**Response Time Scaling:** Linear - Reasonable scalability

## Recommendations

- ⚠️ Response times exceed 1 second at peak load. Consider:
  - Adding caching layer (Redis/Memcached)
  - Optimizing database queries with indexes
  - Implementing pagination for large result sets

