eval $(minikube docker-env)

docker build -t svc-auth:latest -f src/TrueCode.Services.Auth/Dockerfile .
docker build -t svc-currencies:latest -f src/TrueCode.Services.Currencies/Dockerfile .
docker build -t api-gw:latest -f src/TrueCode.API.Web/Dockerfile .
docker build -t svc-fetcher:latest -f src/TrueCode.Services.Fetcher/Dockerfile .
docker build -t job-migrator:latest -f src/TrueCode.Services.Migrator/Dockerfile .

minikube kubectl -- apply -f k8s