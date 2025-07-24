# Resmi Node.js imajını kullanarak build aşaması
FROM node:19.3.0-alpine AS build

WORKDIR /app

COPY package.json ./
COPY package-lock.json ./

RUN npm install

COPY . ./
RUN npm run build

# Build edilen dosyaları serve eden nginx imajı
FROM nginx:stable-alpine

WORKDIR /usr/share/nginx/html
COPY --from=build /app/build .

# Nginx default config dosyasını override et (isteğe bağlı)
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 3000

CMD ["nginx", "-g", "daemon off;"]