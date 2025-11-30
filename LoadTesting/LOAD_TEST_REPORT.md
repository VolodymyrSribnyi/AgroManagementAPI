# AgroManagement API Load Testing Report

## Overview

This document provides a comprehensive analysis of load testing performed on the AgroManagement API. The tests measure the API's performance under various concurrent user loads.

## Test Configuration

### Environment
- **API Server**: AgroManagement API (.NET 8.0)
- **Database**: SQLite with 10,000+ seeded records
- **Test Tool**: Apache jMeter / Python Load Testing Script

### Endpoints Tested
| Endpoint | Method | Description |
|----------|--------|-------------|
| `/api/v1/Resources` | GET | Get all agricultural resources |
| `/api/v1/Workers` | GET | Get all workers |
| `/api/v1/Field/index` | GET | Get all fields |
| `/api/v1/Machines` | GET | Get all machines |
| `/api/v1/Warehouses` | GET | Get all warehouses |

### Test Scenarios
| Scenario | Concurrent Users | Ramp-up Time | Duration |
|----------|------------------|--------------|----------|
| Minimal Load | 1 | 1s | 60s |
| Light Load | 5 | 5s | 60s |
| Normal Load | 20 | 10s | 60s |
| Moderate Load | 50 | 15s | 60s |
| Heavy Load | 100 | 20s | 60s |
| Peak Load | 300 | 30s | 60s |

## Database Seeding

Before running load tests, the database is populated with realistic data using the SeedingApp:

| Table | Records | Data Description |
|-------|---------|------------------|
| Fields | 2,000 | Agricultural fields with area, culture type, status |
| Workers | 2,000 | Workers with names, ages, hourly rates |
| Resources | 2,000 | Agricultural resources with culture-specific data |
| Machines | 2,000 | Farm equipment with types, fuel consumption |
| Warehouses | 500 | Storage facilities |
| InventoryItems | 4,000 | Inventory items with names, quantities, units |
| **Total** | **12,500** | |

## Test Results

### Response Time Analysis

After running the load tests, the results are stored in the `load-test-results/` directory:

```
load-test-results/
├── results_1users.csv
├── results_5users.csv
├── results_20users.csv
├── results_50users.csv
├── results_100users.csv
├── results_300users.csv
├── aggregate_results.csv
├── summary_report.txt
└── load_test_charts.png (if matplotlib available)
```

### Expected Metrics

| Users | Avg Response (ms) | P95 Response (ms) | Error Rate | Throughput |
|-------|-------------------|-------------------|------------|------------|
| 1 | < 100 | < 150 | < 1% | ~10 req/s |
| 5 | < 150 | < 250 | < 1% | ~30 req/s |
| 20 | < 250 | < 400 | < 2% | ~80 req/s |
| 50 | < 400 | < 700 | < 3% | ~150 req/s |
| 100 | < 700 | < 1200 | < 5% | ~200 req/s |
| 300 | < 1500 | < 3000 | < 10% | ~250 req/s |

*Note: Actual results may vary based on hardware and configuration.*

## Response Time Relationship Analysis

The relationship between concurrent users and response time typically follows one of these patterns:

### 1. Linear Relationship
```
Response Time ∝ Users
T(n) = k × n
```
- **Characteristics**: Response time increases proportionally with users
- **Implication**: Reasonable scalability, predictable behavior

### 2. Logarithmic Relationship (Best Case)
```
Response Time ∝ log(Users)
T(n) = k × log(n)
```
- **Characteristics**: Response time increases slowly as users grow
- **Implication**: Excellent scalability, efficient resource usage

### 3. Quadratic Relationship (Concerning)
```
Response Time ∝ Users²
T(n) = k × n²
```
- **Characteristics**: Response time grows faster than user count
- **Implication**: Poor scalability, potential bottlenecks

## How to Run Load Tests

### Prerequisites

1. **Install jMeter** (optional):
   ```bash
   # Download from https://jmeter.apache.org/download_jmeter.cgi
   # Or use package manager:
   apt install jmeter  # Debian/Ubuntu
   brew install jmeter # macOS
   ```

2. **Seed the Database**:
   ```bash
   cd LoadTesting/SeedingApp/SeedingApp
   dotnet run -- "Data Source=../../../AgroindustryAPI.db"
   ```

3. **Start the API Server**:
   ```bash
   cd /path/to/AgroManagementAPI
   dotnet run
   ```

### Running Tests

#### Using Shell Script:
```bash
cd LoadTesting
./run-load-tests.sh --host localhost --port 5000 --duration 60
```

#### Using Python Script:
```bash
cd LoadTesting
python run-load-tests.py --host localhost --port 5000 --duration 60
```

#### Using jMeter Directly:
```bash
jmeter -n -t jmeter-testplan.jmx \
    -Jthreads=50 \
    -Jrampup=10 \
    -Jduration=60 \
    -JresultsFile=load-test-results/results.csv
```

### Quick Test Mode

For a faster test run with fewer user loads:
```bash
./run-load-tests.sh --quick
# or
python run-load-tests.py --quick
```

## Performance Optimization Recommendations

Based on typical load testing results, consider these optimizations:

### Database Layer
1. **Add indexes** on frequently queried columns
2. **Use pagination** for endpoints returning large datasets
3. **Implement caching** for read-heavy operations
4. **Optimize queries** using profiling tools

### Application Layer
1. **Connection pooling** for database connections
2. **Async operations** for I/O-bound tasks
3. **Response compression** (gzip/brotli)
4. **Request rate limiting** to prevent overload

### Infrastructure
1. **Horizontal scaling** with load balancer
2. **Database replication** for read scaling
3. **Caching layer** (Redis/Memcached)
4. **CDN** for static content

## Conclusion

Regular load testing helps identify performance bottlenecks before they affect production. Run these tests:

- Before major releases
- After significant code changes
- Periodically to track performance trends

## Files in This Directory

| File | Description |
|------|-------------|
| `SeedingApp/` | Console application for database seeding |
| `jmeter-testplan.jmx` | jMeter test plan with all scenarios |
| `run-load-tests.sh` | Bash script for running load tests |
| `run-load-tests.py` | Python script for running load tests (with analysis) |
| `load-test-results/` | Directory for test result CSV files |
| `LOAD_TEST_REPORT.md` | This documentation file |
