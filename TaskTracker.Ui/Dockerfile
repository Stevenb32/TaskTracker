FROM node:22 AS build
WORKDIR /app

COPY TaskTracker.Ui/package*.json ./
RUN npm install

COPY TaskTracker.Ui/ ./
RUN npm run build

FROM nginx:alpine

COPY --from=build /app/dist /usr/share/nginx/html
COPY TaskTracker.Ui/nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80