# SQL Otomasyon Proje

Bu proje, C# Windows Forms ve SQL Server kullanilarak gelistirilmis bir kutuphane otomasyon uygulamasidir.

Uygulama; kullanici girisi, kitap listeleme, kiralama islemleri, personel/kullanici yonetimi, odeme takibi ve veritabani baglantisi gibi temel kutuphane otomasyon ozelliklerini icerir.

## Icerik

- `KutuphaneOtomasyon/`: C# Windows Forms kaynak kodlari.
- `Database/`: SQL Server veritabani yedekleri ve sema ozeti.
- `PowerBI/`: Kutuphane otomasyonu icin hazirlanmis Power BI dashboard dosyasi.

## Teknolojiler

- C#
- Windows Forms
- .NET
- SQL Server
- Microsoft.Data.SqlClient
- Power BI

## Power BI Dashboard

`PowerBI/kutuphane-dashboard.pbix` dosyasi, kutuphane otomasyon projesindeki verilerin Power BI uzerinde raporlanmasi ve gorsellestirilmesi icin hazirlanmistir.

Dashboard; kutuphane sistemiyle ilgili verilerin daha okunabilir sekilde incelenmesini, raporlanmasini ve yonetim tarafinda hizli fikir vermesini amaclar. Power BI Desktop ile acilarak grafikler, tablolar ve rapor sayfalari uzerinden detayli analiz yapilabilir.

## Not

`bin`, `obj`, yayin ciktilari ve kullaniciya ozel Visual Studio ayarlari GitHub'a dahil edilmemistir. Proje Visual Studio ile acilip gerekli NuGet paketleri geri yuklenerek derlenebilir.
