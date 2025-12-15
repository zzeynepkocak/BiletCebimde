# Test Senaryoları Rehberi

## Senaryo 1: Organizatörün Kendi Etkinliğini Oluşturup Kontrol Etmesi

### Adım 1: Organizatör Olarak Giriş Yapın
1. Uygulamayı çalıştırın
2. `Login` sayfasına gidin
3. Organizatör hesabı ile giriş yapın
   - Eğer organizatör hesabı yoksa, yeni bir kullanıcı oluşturun ve `Organizer` rolü verin

### Adım 2: Yeni Etkinlik Oluşturma
1. Dashboard veya navigasyon menüsünden **"Yeni Etkinlik Oluştur"** butonuna tıklayın
2. Etkinlik bilgilerini doldurun:
   - **Başlık**: "C# Atölyesi"
   - **Açıklama**: "C# programlama dili atölye çalışması"
   - **Tarih**: Gelecek bir tarih seçin
   - **Saat**: Bir saat seçin
   - **Kontenjan**: 5
   - **Kategori**: Bir kategori seçin (Admin tarafından oluşturulmuş olmalı)
   - **Mekan**: Bir mekan seçin (Admin tarafından oluşturulmuş olmalı)
3. **"Oluştur"** butonuna tıklayın
4. ✅ **Beklenen Sonuç**: Etkinlik başarıyla oluşturulmalı ve detay sayfasına yönlendirilmelisiniz

### Adım 3: MyEvents (Kendi Etkinlikleri) Kontrolü
1. Navigasyon menüsünden **"Dashboard"** veya **"Etkinliklerim"** linkine tıklayın
   - Alternatif: `Events/MyEvents` URL'sine gidin
2. ✅ **Beklenen Sonuç**: 
   - Yeni oluşturduğunuz "C# Atölyesi" etkinliğini görmelisiniz
   - Etkinlik bilgileri (tarih, kategori, mekan, kontenjan) görünür olmalı
   - Kontenjan durumu: 0/5 olarak görünmeli

### Adım 4: Katılımcı Listesi Kontrolü
1. MyEvents sayfasında "C# Atölyesi" etkinliğinin yanındaki **"Katılımcılar"** butonuna tıklayın
   - Alternatif: Etkinlik detay sayfasından "Katılımcılar" linkine tıklayın
2. ✅ **Beklenen Sonuç**: 
   - Katılımcı listesi sayfası açılmalı
   - "Bu etkinliğe henüz katılımcı kaydı yapılmamış" mesajı görünmeli
   - Liste boş olmalı

---

## Senaryo 2: Kullanıcının Kayıt Olması ve Kontenjan Kontrolü

### Adım 1: Kullanıcı Olarak Giriş Yapın
1. Mevcut oturumu kapatın (Logout)
2. Farklı bir kullanıcı hesabı ile giriş yapın (User rolü)
   - Eğer kullanıcı hesabı yoksa, yeni bir kullanıcı oluşturun

### Adım 2: Etkinlik Listeleme ve Detay Görüntüleme
1. Navigasyon menüsünden **"Etkinlikler"** linkine tıklayın
2. Organizatörün oluşturduğu **"C# Atölyesi"** etkinliğini bulun
3. Etkinlik kartındaki **"Detaylar"** butonuna tıklayın
4. ✅ **Beklenen Sonuç**: 
   - Etkinlik detay sayfası açılmalı
   - Tüm etkinlik bilgileri görünür olmalı
   - "Etkinliğe Katıl" butonu görünür olmalı (kontenjan dolmamışsa)

### Adım 3: Kayıt Olma
1. Etkinlik detay sayfasında **"Etkinliğe Katıl"** butonuna tıklayın
2. ✅ **Beklenen Sonuç**: 
   - "Etkinliğe başarıyla kayıt oldunuz!" mesajı görünmeli
   - Kontenjan durumu 1/5 olarak güncellenmiş olmalı
   - Buton "Kaydı İptal Et" olarak değişmeli

### Adım 4: MyRegistrations (Kayıtlarım) Kontrolü
1. Navigasyon menüsünden **"Profilim"** veya **"Kayıtlarım"** linkine tıklayın
   - Alternatif: `Registrations/MyRegistrations` URL'sine gidin
2. ✅ **Beklenen Sonuç**: 
   - "C# Atölyesi" etkinliğini listede görmelisiniz
   - Kayıt tarihi görünür olmalı
   - Etkinlik detaylarına gidebileceğiniz link olmalı

### Adım 5: Kontenjan Kontrolü (Hata Kontrolü)
1. **4 farklı kullanıcı daha** aynı etkinliğe kayıt olsun (Toplam 5 kayıt)
   - Her kullanıcı için:
     - Logout yapın
     - Farklı bir kullanıcı ile giriş yapın
     - Etkinlik detay sayfasına gidin
     - "Etkinliğe Katıl" butonuna tıklayın
2. **5. kayıt yapıldıktan sonra**:
   - Kontenjan durumu 5/5 olmalı
   - "Kontenjan Dolu" badge'i görünmeli
3. **6. kullanıcı kayıt olmaya çalıştığında**:
   - ✅ **Beklenen Sonuç**: 
     - "Bu etkinliğin kontenjanı dolmuştur" hata mesajı görünmeli
     - Kayıt işlemi gerçekleşmemeli
     - "Etkinliğe Katıl" butonu görünmemeli (veya devre dışı olmalı)
     - "Kontenjan dolmuştur" uyarısı görünmeli

### Adım 6: Organizatörün Katılımcı Listesini Kontrol Etmesi
1. Organizatör hesabı ile tekrar giriş yapın
2. MyEvents sayfasına gidin
3. "C# Atölyesi" etkinliğinin yanındaki **"Katılımcılar"** butonuna tıklayın
4. ✅ **Beklenen Sonuç**: 
   - 5 katılımcı listelenmeli
   - Her katılımcının adı, e-postası ve kayıt tarihi görünmeli
   - Liste sıralı olmalı (kayıt tarihine göre)

---

## Ek Test Senaryoları

### Senaryo 3: Aynı Kullanıcının Tekrar Kayıt Olmaya Çalışması
1. Zaten kayıtlı olduğunuz bir etkinliğe tekrar kayıt olmaya çalışın
2. ✅ **Beklenen Sonuç**: 
   - "Bu etkinliğe zaten kayıtlısınız" hata mesajı görünmeli
   - Kayıt işlemi gerçekleşmemeli

### Senaryo 4: Geçmiş Etkinliğe Kayıt Olmaya Çalışma
1. Geçmiş bir tarihte olan bir etkinliğe kayıt olmaya çalışın
2. ✅ **Beklenen Sonuç**: 
   - "Geçmiş etkinliklere kayıt olamazsınız" hata mesajı görünmeli
   - Kayıt butonu görünmemeli

### Senaryo 5: Organizatörün Kendi Etkinliğine Kayıt Olmaya Çalışması
1. Organizatör olarak kendi oluşturduğunuz etkinliğe kayıt olmaya çalışın
2. ✅ **Beklenen Sonuç**: 
   - Sistem izin vermeli (isteğe bağlı - mevcut kodda engellenmemiş)
   - Veya "Kendi etkinliğinize kayıt olamazsınız" mesajı (eğer bu kural eklendiyse)

---

## Test Sonuçları

Her senaryo için:
- ✅ **Başarılı**: Beklenen sonuç gerçekleşti
- ❌ **Başarısız**: Beklenen sonuç gerçekleşmedi (hata detaylarını not edin)
- ⚠️ **Kısmen Başarılı**: Bazı özellikler çalışıyor ama eksikler var

---

## Notlar

- Test sırasında hata alırsanız, hata mesajlarını not edin
- Tarayıcı konsolunda (F12) JavaScript hataları olup olmadığını kontrol edin
- Veritabanı durumunu kontrol etmek için SQL Server Management Studio kullanabilirsiniz

