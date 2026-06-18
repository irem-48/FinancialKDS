# FinancialKDS - Finansal Karar Destek Sistemi

FinancialKDS, yatırım projelerinin fizibilite analizini gerçekleştirmek amacıyla geliştirilmiş web tabanlı bir karar destek sistemidir. Sistem, kullanıcı tarafından girilen geçmiş dönem nakit akışı verilerini doğrusal regresyon yöntemiyle analiz eder, gelecek dönem nakit akışlarını tahmin eder ve Net Bugünkü Değer (NPV) hesaplaması ile yatırım kararını destekler.

## Projenin Amacı

Bu projenin temel amacı, yatırım projelerinin finansal değerlendirme sürecini dijitalleştirerek manuel hesaplama hatalarını azaltmak ve kullanıcıya hızlı, anlaşılır ve grafik destekli fizibilite raporları sunmaktır.

Sistem sayesinde kullanıcılar:

* Yeni yatırım projesi oluşturabilir.
* Projeye ait ilk yatırım maliyeti, iskonto oranı ve tahmin dönemi bilgilerini girebilir.
* Geçmiş dönem nakit akışı verilerini kaydedebilir.
* Doğrusal regresyon ile gelecek dönem nakit akışlarını tahmin edebilir.
* NPV hesaplaması ile yatırımın finansal açıdan kabul edilebilir olup olmadığını görebilir.
* Analiz sonuçlarını grafiksel olarak inceleyebilir.
* Rapor çıktısını PDF olarak alabilir.

## Kullanılan Teknolojiler

* C#
* ASP.NET Core MVC
* Entity Framework Core
* Microsoft SQL Server LocalDB
* HTML5
* CSS3
* Bootstrap
* JavaScript
* Chart.js
* Visual Studio 2022

## Proje Özellikleri

### Proje Yönetimi

Kullanıcı sistem üzerinde yeni yatırım projeleri oluşturabilir, mevcut projeleri düzenleyebilir ve silebilir. Her proje için proje adı, açıklama, ilk yatırım maliyeti, iskonto oranı ve tahmin edilecek dönem sayısı bilgileri tutulur.

### Nakit Akışı Yönetimi

Her yatırım projesi için geçmiş dönem nakit akışı verileri sisteme girilebilir. Sistem, aynı proje için aynı dönem bilgisinin tekrar girilmesini engelleyerek veri bütünlüğünü korur.

### Doğrusal Regresyon ile Tahminleme

Girilen geçmiş dönem nakit akışı verileri doğrusal regresyon yöntemiyle analiz edilir. Sistem, regresyon katsayılarını hesaplayarak gelecek dönemler için tahmini nakit akışları üretir.

### NPV Hesaplama

Tahmin edilen gelecek dönem nakit akışları, kullanıcının belirlediği iskonto oranı ile bugünkü değere indirgenir. Elde edilen toplam bugünkü değerden ilk yatırım maliyeti çıkarılarak Net Bugünkü Değer hesaplanır.

NPV sonucuna göre sistem şu kararları üretir:

* NPV > 0 ise yatırım finansal açıdan kabul edilebilir.
* NPV < 0 ise yatırım finansal açıdan uygun değildir.
* NPV = 0 ise yatırım başa baş noktasındadır.

### Grafiksel Raporlama

Analiz sonuçları tablo ve grafiklerle kullanıcıya sunulur. Chart.js kullanılarak geçmiş ve tahmini nakit akışları karşılaştırmalı olarak görselleştirilir.

### PDF / Yazdırılabilir Rapor

Analiz sonuçları, tarayıcının yazdırma özelliği kullanılarak PDF formatında raporlanabilir. Bu çıktı, yatırım fizibilite raporu olarak kullanılabilir.

## Proje Yapısı

```text
FinancialKDS
│
├── Controllers
│   ├── ProjectsController.cs
│   └── CashFlowsController.cs
│
├── Data
│   └── ApplicationDbContext.cs
│
├── Models
│   ├── Project.cs
│   ├── CashFlow.cs
│   ├── ForecastResult.cs
│   └── AnalysisResult.cs
│
├── Services
│   ├── RegressionService.cs
│   └── NpvService.cs
│
├── Views
│   ├── Projects
│   ├── CashFlows
│   └── Shared
│
├── wwwroot
├── Program.cs
└── appsettings.json
```

## Veritabanı Yapısı

Projede Entity Framework Core Code First yaklaşımı kullanılmıştır. Temel tablolar şunlardır:

* Projects
* CashFlows
* ForecastResults
* AnalysisResults

## Kurulum

Projeyi çalıştırmak için aşağıdaki adımlar izlenebilir:

1. Repoyu bilgisayara klonlayın.

```bash
git clone https://github.com/kullanici-adiniz/FinancialKDS.git
```

2. Proje klasörüne girin.

```bash
cd FinancialKDS
```

3. Visual Studio 2022 ile `FinancialKDS.sln` dosyasını açın.

4. NuGet paketlerinin yüklü olduğundan emin olun.

Kullanılan temel paketler:

```text
Microsoft.EntityFrameworkCore.SqlServer
Microsoft.EntityFrameworkCore.Tools
Microsoft.EntityFrameworkCore.Design
```

5. Veritabanını oluşturmak için Package Manager Console üzerinden şu komutu çalıştırın:

```powershell
Update-Database
```

6. Projeyi çalıştırın.

```text
https
```

veya Visual Studio içerisinden yeşil çalıştırma butonuna basın.

## Örnek Kullanım Senaryosu

Örnek bir yatırım projesi için aşağıdaki bilgiler girilebilir:

```text
Proje Adı: E-Ticaret Depo Otomasyonu Yatırımı
İlk Yatırım Maliyeti: 750000
İskonto Oranı: 20
Tahmin Dönemi: 5
```

Geçmiş nakit akışları:

```text
Dönem 1: 160000
Dönem 2: 210000
Dönem 3: 260000
Dönem 4: 320000
```

Sistem bu verilere göre gelecek dönem nakit akışlarını tahmin eder, NPV değerini hesaplar ve yatırım kararını kullanıcıya sunar.

## Geliştirici

İrem Nisa Sözen
Yazılım Mühendisliği
Manisa Celal Bayar Üniversitesi
