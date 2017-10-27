# NoobOrchard Repositories测试数据

``` ini

BenchmarkDotNet=v0.10.9, OS=Windows 10 Redstone 2 (10.0.15063)
Processor=AMD Athlon(tm) II X3 435 Processor, ProcessorCount=3
Frequency=2844888 Hz, Resolution=351.5077 ns, Timer=TSC
  [Host]     : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2115.0 DEBUG
  Job-PKMTER : .NET Framework 4.7 (CLR 4.0.30319.42000), 32bit LegacyJIT-v4.7.2115.0

RemoveOutliers=True  LaunchCount=1  TargetCount=5  
UnrollFactor=5  WarmupCount=0  

```
 |                 ORM |                                               Namespace |                         Type |                   Method |         Mean |        Error |       StdDev |     Gen 0 |   Gen 1 |   Gen 2 | Allocated |
 |-------------------- |-------------------------------------------------------- |----------------------------- |------------------------- |-------------:|-------------:|-------------:|----------:|--------:|--------:|----------:|
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |              SingleAsync |     5.222 ms |     9.771 ms |     2.538 ms |  495.0000 |       - |       - |  145216 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |              SingleAsync |    69.395 ms |   151.930 ms |    39.463 ms |  580.0000 |       - |       - |  522811 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |              SingleAsync |    23.595 ms |     4.948 ms |     1.285 ms | 1305.0000 | 25.0000 |       - |  466036 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |              SingleAsync |    17.200 ms |     6.893 ms |     1.791 ms |  340.0000 |       - |       - |  104217 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |    SingleAsync_Predicate |    49.764 ms |   106.038 ms |    27.543 ms |  542.5000 |  7.5000 |       - |  158272 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |    SingleAsync_Predicate |    78.195 ms |    45.407 ms |    11.794 ms |  640.0000 |       - |       - |  222696 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |    SingleAsync_Predicate |    26.680 ms |    19.296 ms |     5.012 ms |  850.0000 |       - |       - |  251040 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |    SingleAsync_Predicate |    19.889 ms |     8.954 ms |     2.326 ms |  375.0000 |       - |       - |  106068 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |             GetListAsync |    16.467 ms |    42.442 ms |    11.024 ms |  522.5000 | 12.5000 |       - |  160638 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |             GetListAsync |   246.457 ms |   320.675 ms |    83.294 ms |  600.0000 |       - |       - |  218258 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |             GetListAsync |    39.420 ms |    98.978 ms |    25.709 ms |  865.0000 |       - |       - |  259332 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |             GetListAsync |    61.609 ms |    76.550 ms |    19.884 ms |  300.0000 |       - |       - |   89072 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |      GetPaggingListAsync |    18.300 ms |    69.239 ms |    17.984 ms |  427.5000 | 25.0000 |       - |  196131 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |      GetPaggingListAsync |   199.388 ms |   232.048 ms |    60.274 ms |  640.0000 |       - |       - |  278236 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |      GetPaggingListAsync |    23.863 ms |    29.003 ms |     7.533 ms |  930.0000 |       - |       - |  284875 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |      GetPaggingListAsync |    22.890 ms |    32.320 ms |     8.395 ms |  640.0000 |       - |       - |  219351 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |                   Exists |   179.776 ms |   104.686 ms |    27.192 ms |  200.0000 |       - |       - |   91471 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |                   Exists | 1,375.812 ms | 1,949.946 ms |   506.491 ms |  640.0000 | 80.0000 | 80.0000 |  180123 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |                   Exists |   183.287 ms |    24.327 ms |     6.319 ms |  320.0000 |       - |       - |  106183 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |                   Exists | 1,676.501 ms | 4,130.955 ms | 1,073.000 ms |  200.0000 |       - |       - |   70864 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |              ExistsAsync |   210.169 ms |   160.472 ms |    41.682 ms |  200.0000 |       - |       - |  103884 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |              ExistsAsync |   798.955 ms |   948.635 ms |   246.404 ms |  600.0000 |       - |       - |  193212 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |              ExistsAsync |   332.897 ms |   479.104 ms |   124.446 ms |  400.0000 |       - |       - |  123085 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |              ExistsAsync |   438.546 ms |   341.223 ms |    88.631 ms |  200.0000 |       - |       - |   85960 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |              InsertAsync |     5.672 ms |    11.353 ms |     2.949 ms |  312.5000 |       - |       - |   83638 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |              InsertAsync |   113.387 ms |   236.521 ms |    61.436 ms |  360.0000 |       - |       - |  156062 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |              InsertAsync |    12.890 ms |    18.304 ms |     4.754 ms | 1285.0000 |  7.5000 |       - |  364616 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |              InsertAsync |   119.580 ms |   249.394 ms |    64.779 ms |  220.0000 |       - |       - |   78402 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |      InsertAndGetIdAsync |    17.067 ms |    44.004 ms |    11.430 ms |  300.0000 |       - |       - |   83700 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |      InsertAndGetIdAsync |   200.090 ms |   406.189 ms |   105.506 ms |  400.0000 |       - |       - |  157652 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |      InsertAndGetIdAsync |    15.489 ms |     5.974 ms |     1.552 ms |  785.0000 |       - |       - |  234603 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |      InsertAndGetIdAsync |   145.081 ms |   164.628 ms |    42.761 ms |  200.0000 |       - |       - |   81378 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |          InsertListAsync |     3.415 ms |     7.305 ms |     1.897 ms |  342.5000 |       - |       - |   91035 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |          InsertListAsync |   217.096 ms |   461.867 ms |   119.968 ms |  360.0000 |       - |       - |  155597 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |          InsertListAsync |     7.874 ms |     8.183 ms |     2.125 ms | 1307.5000 |  7.5000 |       - |  366834 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |          InsertListAsync |    60.788 ms |   161.456 ms |    41.938 ms |  260.0000 |       - |       - |   80475 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |              UpdateAsync |     7.895 ms |    10.277 ms |     2.669 ms |  542.5000 |       - |       - |  148998 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |              UpdateAsync |   236.543 ms |   280.997 ms |    72.988 ms |  800.0000 |       - |       - |  253718 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |              UpdateAsync |    21.721 ms |    21.361 ms |     5.548 ms | 1910.0000 |  5.0000 |       - |  538280 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |              UpdateAsync |   146.977 ms |   204.217 ms |    53.045 ms |  300.0000 |       - |       - |  103508 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |    UpdateAsync_Predicate |           NA |           NA |           NA |       N/A |     N/A |     N/A |       N/A |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |    UpdateAsync_Predicate |   546.597 ms |   587.122 ms |   152.503 ms |  640.0000 |       - |       - |  222674 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |    UpdateAsync_Predicate |   288.537 ms | 1,410.696 ms |   366.423 ms | 1000.0000 |       - |       - |  352010 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |    UpdateAsync_Predicate |   214.544 ms |   102.401 ms |    26.598 ms |  200.0000 |       - |       - |   87863 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |              DeleteAsync |     5.642 ms |     7.280 ms |     1.891 ms |  375.0000 |       - |       - |   99594 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |              DeleteAsync |   571.015 ms |   893.768 ms |   232.153 ms |  600.0000 |       - |       - |  180973 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |              DeleteAsync |   150.032 ms |   173.904 ms |    45.171 ms |  400.0000 |       - |       - |  110533 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |              DeleteAsync |   224.624 ms |   224.284 ms |    58.257 ms |  280.0000 |       - |       - |  105484 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |           DeleteAsync_Id |     5.192 ms |     4.055 ms |     1.053 ms |  378.7500 |       - |       - |  100910 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |           DeleteAsync_Id |   187.052 ms |   478.306 ms |   124.238 ms |  600.0000 |       - |       - |  181545 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |           DeleteAsync_Id |   222.702 ms |   380.497 ms |    98.833 ms |  400.0000 |       - |       - |  111584 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |           DeleteAsync_Id |   182.045 ms |   262.445 ms |    68.169 ms |  260.0000 |       - |       - |   80556 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |    DeleteAsync_Predicate |     8.543 ms |    10.605 ms |     2.755 ms |  420.0000 |       - |       - |  111340 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |    DeleteAsync_Predicate |   195.564 ms |   238.900 ms |    62.053 ms |  680.0000 |       - |       - |  221327 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |    DeleteAsync_Predicate |   251.499 ms |   256.685 ms |    66.673 ms |  680.0000 |       - |       - |  221889 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |    DeleteAsync_Predicate |   134.958 ms |   250.343 ms |    65.026 ms |  200.0000 |       - |       - |   84276 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |               CountAsync |   192.320 ms |   112.287 ms |    29.166 ms |  200.0000 |       - |       - |  103227 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |               CountAsync |   211.132 ms |   121.751 ms |    31.624 ms |  600.0000 |       - |       - |  194469 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |               CountAsync |   575.471 ms |   635.339 ms |   165.027 ms |  400.0000 |       - |       - |  121073 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |               CountAsync |   320.283 ms |   225.063 ms |    58.459 ms |  200.0000 |       - |       - |   84025 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |     CountAsync_Predicate |   170.928 ms |    79.178 ms |    20.566 ms |  200.0000 |       - |       - |  104206 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |     CountAsync_Predicate |   176.971 ms |    33.307 ms |     8.651 ms |  600.0000 |       - |       - |  194750 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |     CountAsync_Predicate |   257.905 ms |   283.636 ms |    73.673 ms |  400.0000 |       - |       - |  123702 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |     CountAsync_Predicate |   344.396 ms |   276.617 ms |    71.850 ms |  200.0000 |       - |       - |   86609 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks |           LongCountAsync |   181.132 ms |    19.270 ms |     5.005 ms |  200.0000 |       - |       - |  101997 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks |           LongCountAsync |   164.798 ms |    65.462 ms |    17.003 ms |  520.0000 |       - |       - |  191853 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks |           LongCountAsync |   183.175 ms |   126.324 ms |    32.812 ms |  400.0000 |       - |       - |  122396 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks |           LongCountAsync |   491.729 ms |   293.564 ms |    76.252 ms |  200.0000 |       - |       - |   84277 B |
 |              Dapper |              Orchard.Repositories.BenchmarkTests.Dapper |  AdmAreaTestDapperBenchmarks | LongCountAsync_Predicate |   464.003 ms |   659.535 ms |   171.312 ms |  200.0000 |       - |       - |  103924 B |
 |     EntityFramework |     Orchard.Repositories.BenchmarkTests.EntityFramework |      AdmAreaTestEfBenchmarks | LongCountAsync_Predicate |   170.595 ms |    62.519 ms |    16.239 ms |  560.0000 |       - |       - |  195208 B |
 | EntityFrameworkCore | Orchard.Repositories.BenchmarkTests.EntityFrameworkCore |  AdmAreaTestEfCoreBenchmarks | LongCountAsync_Predicate |   262.972 ms |   343.698 ms |    89.274 ms |  400.0000 |       - |       - |  122704 B |
 |             OrmLite |             Orchard.Repositories.BenchmarkTests.OrmLite | AdmAreaTestOrmLiteBenchmarks | LongCountAsync_Predicate |   377.902 ms |   154.412 ms |    40.108 ms |  200.0000 |       - |       - |   86883 B |

Benchmarks with issues:
  AdmAreaTestDapperBenchmarks.UpdateAsync_Predicate: Job-PKMTER(RemoveOutliers=True, LaunchCount=1, TargetCount=5, UnrollFactor=5, WarmupCount=0)
