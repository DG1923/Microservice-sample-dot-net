# Microservices Project with Kubernetes

## Prerequisites
- Docker Desktop with Kubernetes enabled
- kubectl CLI
- SQL Server
- RabbitMQ

## Setup Steps

### 1. Install Ingress Controller
```bash
# Install NGINX Ingress Controller
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.1/deploy/static/provider/cloud/deploy.yaml

# Verify installation
kubectl get pods -n ingress-nginx
kubectl get services -n ingress-nginx
```

### 2. Deploy SQL Server
```bash
# Create SQL Server credentials
kubectl create secret generic mssql --from-literal=SA_PASSWORD="Abc123!@#"

# Deploy SQL Server with persistent volume
kubectl apply -f local_pvc.yaml
```

### 3. Deploy RabbitMQ
```bash
# Deploy RabbitMQ message broker
kubectl apply -f rabbitmq-depl.yaml
```

### 4. Deploy Microservices
```bash
# Deploy Platform Service
kubectl apply -f platform-depl.yaml

# Deploy Commands Service
kubectl apply -f command-depl.yaml

# Deploy Ingress rules
kubectl apply -f ingress-srv.yaml
```

## Useful Commands

### Deployment Management
```bash
# List all deployments
kubectl get deployments

# List all pods
kubectl get pods

# List all services
kubectl get services
```

### Deployment Operations
```bash
# Restart a deployment
kubectl rollout restart deployment platform-depl

# Check deployment status
kubectl rollout status deployment platform-depl

# View deployment history
kubectl rollout history deployment platform-depl
```

### Delete Deployments
```bash
# Remove platform deployment
kubectl delete deployment platform-depl

# Remove command deployment
kubectl delete deployment command-depl
```

## Troubleshooting

### Check Resources
```bash
# Check pod status
kubectl get pods

# Check pod logs
kubectl logs <pod-name>

# Describe pod details
kubectl describe pod <pod-name>
```

### Check Services
```bash
# List all services
kubectl get services

# Check ingress status
kubectl get ingress
```

## Architecture
- Platform Service: Manages platform data
- Commands Service: Handles command processing
- SQL Server: Primary database
- RabbitMQ: Message broker for async communication
- NGINX Ingress: API Gateway and routing