# AgroSolutions Functions ⚡🌱
Este repositório contém as funções de processamento em segundo plano e lógica baseada em eventos da plataforma AgroSolutions. O projeto foi desenvolvido para lidar com tarefas assíncronas e processamento de dados do setor agrícola, 
utilizando conteinerização com Docker e deploy automatizado para o Amazon EKS (Elastic Kubernetes Service).

# 🚀 Tecnologias Utilizadas
 - Linguagem: .NET Core / C# (Azure Functions Core Tools ou Worker Services)

 - Containerização: Docker

 - Cloud Provider: AWS (Amazon Web Services)

 - Orquestração: Kubernetes (Amazon EKS)

 - Registro de Imagens: Amazon ECR (Elastic Container Registry)

 - CI/CD: GitHub Actions

 - Integração: Mensageria / Eventos (conforme configuração de trigger)

# 🏗️ Arquitetura de Deploy (CI/CD)
O projeto utiliza o GitHub Actions para garantir que cada atualização de lógica seja testada e enviada automaticamente para o cluster:

Checkout: Coleta a versão mais recente das funções.

 1. AWS Auth: Autentica no ambiente AWS usando Secrets.
 2. 2. Docker Build & Push: Gera a imagem do Worker/Function e envia para o Amazon ECR.
 3. Kubernetes Config: Configura o contexto do kubectl para o cluster EKS.
 4. Secrets Management: Sincroniza as chaves de acesso e strings de conexão necessárias para o processamento.
 5. Rolling Update: Atualiza o Deployment/CronJob no EKS para processar as tarefas com a nova lógica.

# 📦 Como rodar localmente (Docker)
Para testar as funções localmente dentro de um container:
```
# Build da imagem
docker build -t agrosolutions-functions .

# Rodar o container
docker run -d --name agrosolutions-functions agrosolutions-functions
```
# ☸️ Deploy no Kubernetes
```
# Aplicar configurações de Worker/Function
kubectl apply -f k8s/deployment.yaml
```
