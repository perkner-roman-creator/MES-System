# React + Vite frontend

# Stage 1: Build
FROM node:18-alpine AS builder
WORKDIR /app

# Kopíruj package files
COPY package*.json ./

# Instaluj dependencies
RUN npm ci

# Kopíruj source code
COPY . .

# Build pro production
RUN npm run build

# Stage 2: Serve
FROM node:18-alpine
WORKDIR /app

# Instaluj lightweight server
RUN npm install -g serve

# Kopíruj built aplikaci
COPY --from=builder /app/dist ./dist

# Health check
HEALTHCHECK --interval=10s --timeout=3s --start-period=5s --retries=3 \
  CMD wget --quiet --tries=1 --spider http://localhost:3000/ || exit 1

# Expose port
EXPOSE 3000

# Start aplikace
CMD ["serve", "-s", "dist", "-l", "3000"]
