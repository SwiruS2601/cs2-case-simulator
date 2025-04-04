ARG NODE_VERSION=22.14.0

FROM node:${NODE_VERSION}-slim AS build

WORKDIR /app
COPY ./package.json /app/
RUN npm install
COPY . ./

RUN npm run build

FROM node:${NODE_VERSION}-slim

WORKDIR /app

COPY --from=build /app/.output ./

ENV NITRO_PORT=5173
ENV NITRO_HOST=0.0.0.0
ENV NODE_ENV=production

EXPOSE 5173

CMD ["node","/app/server/index.mjs"]