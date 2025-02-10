# Тестовое задание для компании TrueCode

## Сервисы
1. **TrueCode.Services.Migrator** - сервис миграции БД. Работает как Job в Kubernetes (срабатывает единожды при деплое/редеплое)
2. **TrueCode.Services.Fetcher** - сервис получения данных о валютах из API ЦБ РФ.
3. **TrueCode.Services.Auth** - сервис регистрации и аутентификации пользователей. Реализован как gRPC-микросервис с JWT-авторизацией.
4. **TrueCode.Services.Currencies** - сервис, позволяющий пользователю добавлять валюты в favorites и просматривать их список. Реализован как gRPC-микросервис с JWT-авторизацией.
5. **TrueCode.API.Web** - API Gateway, выполненный как web-сервис, служащий клиентом для gRPC-сервисов. В реальном проекте я бы, скорее, использовал Envoy или другое подобное решение, но, полагаю, для тестового задания достаточно и такой реализации.

## Сборка и локальный запуск

1. Требуются установленные [Docker Engine](https://www.docker.com) и [minikube](https://minikube.sigs.k8s.io/docs/start/?arch=%2Fwindows%2Fx86-64%2Fstable%2F.exe+download)
2. Включить аддон Ingress в minikube: `minikube addons enable ingress`
3. Добавить IP-адрес minikube-кластера Kubernetes в hosts: `echo "$(minikube ip) localhost" | sudo tee -a /etc/hosts`
4. Туннелировать Ingress наружу: `minikube tunnel`. 
5. API Gateway будет доступен по адресу http://localhost.
6. Документация OpenAPI будет доступна по адресу http://localhost/swagger/index.html