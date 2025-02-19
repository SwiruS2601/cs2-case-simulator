FROM oven/bun:1 AS build
WORKDIR /app

# Install required system dependencies
RUN apt-get update && apt-get install -y \
    procps \
    && rm -rf /var/lib/apt/lists/*

# Copy package files
COPY package.json ./
COPY bun.lock ./
COPY tsconfig*.json ./
COPY vite.config.ts ./

# Install dependencies
RUN bun install --frozen-lockfile

# Copy source files
COPY src/ ./src/
COPY public/ ./public/
COPY index.html ./
COPY .env.production ./.env

# Type check and build
RUN bun run build

# Production stage
FROM nginx:stable-alpine
WORKDIR /usr/share/nginx/html

# Copy built files from build stage
COPY --from=build /app/dist .

COPY nginx.conf /etc/nginx/conf.d/default.conf

# Expose port 80
EXPOSE 80

# Start Nginx
CMD ["nginx", "-g", "daemon off;"]