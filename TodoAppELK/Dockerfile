# === Build Stage ===
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# .dockerignore dosyası ile sadece gerekli dosyaların kopyalandığından emin ol.

# 1. Sadece proje dosyalarını kopyala ve bağımlılıkları yükle (Layer Caching)
COPY *.csproj ./
RUN dotnet restore

# 2. Kaynak kodunun geri kalanını kopyala
COPY . .

# 3. Uygulamayı Release modunda publish et
# Sadece tek bir publish komutu yeterli
RUN dotnet publish -c Release -o /app/publish

# === Runtime Stage ===
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Uygulamanın hangi portu dinleyeceğini belirt (önerilir)
ENV ASPNETCORE_URLS=http://+:5159
EXPOSE 5159

# Yayınlanmış dosyaları build aşamasından kopyala
COPY --from=build /app/publish .

# Uygulamayı başlat
ENTRYPOINT ["dotnet", "TodoAppELK.dll"]