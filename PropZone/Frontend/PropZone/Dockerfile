# Stage 1: Build Angular App
FROM node:20-alpine AS builder

WORKDIR /app

COPY package.json package-lock.json ./
RUN npm install --legacy-peer-deps

COPY . .
RUN npm run build -- --configuration production

# Stage 2: Serve with NGINX
FROM nginx:1.25-alpine

# Clean default NGINX content
RUN rm -rf /usr/share/nginx/html/*

# Copy Angular output to NGINX public folder
COPY --from=builder /app/dist/PropZone/browser /usr/share/nginx/html

# custom NGINX config (for Angular routing fallback)
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]



