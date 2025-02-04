FROM node:23.7

# Set working directory in the container
WORKDIR /app

# Copy package.json and package-lock.json (or yarn.lock) first
# to take advantage of cached Docker layers for dependency installation.
COPY src/DemoWebsite/package*.json ./

# Install dependencies
RUN npm install

# Install the Angular CLI globally.
# If you specifically want to lock to a certain Angular CLI version,
RUN npm install -g @angular/cli@latest

# Copy the rest of your project files into the container
COPY src/DemoWebsite/ ./

# Expose Angular's default dev-server port
EXPOSE 57391

# Run ng serve on container startup. 
# Host 0.0.0.0 ensures the server is accessible outside the container.
CMD ["ng", "serve", "--host", "0.0.0.0", "--port", "57391", "--proxy-config", "proxy.conf.json"]]
