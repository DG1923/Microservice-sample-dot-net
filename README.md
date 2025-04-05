This is a project follow tutorial on youtube 
# deploy kubernetes 
kubectl apply -f platform-depl.yaml

# Get deployments in kubernetes
kubectl get deployments

# Get pods in kubernetes
kubectl get pods

# Get services in kubernetes
kubectl get services

# kill the deployment
kubectl delete deployment platform-depl
kubectl delete deployment command-depl

# restart the deployment
kubectl rollout restart deployment platform-depl

# install ingress nginx 
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.12.1/deploy/static/provider/cloud/deploy.yaml
# get pods as namespace = ingress-nginx
kubectl get pods -n ingress-nginx

# get services as namespace = ingress-nginx
kubectl get services -n ingress-nginx

# deploy the ingress service
kubectl apply -f ingress-srv.yaml

# deploy sql server
kubectl apply -f local_pvc.yaml

# create account for sql server
kubectl create secret generic mssql --from-literal=SA_PASSWORD="Abc123!@#"

# Deployment commands
kubectl rollout restart deployment platform-depl    # Restart a deployment
kubectl rollout status deployment platform-depl     # Check rollout status
kubectl rollout history deployment platform-depl    # View rollout history

# Verify deployment
kubectl get deployments                            # List all deployments
kubectl get pods                                   # Check if pods are running