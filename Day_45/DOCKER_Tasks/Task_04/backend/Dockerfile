# Use Node.js base image
FROM node:18

# Set working directory inside container
WORKDIR /app

# Copy package files first for Docker layer caching
COPY package*.json ./

# Install dependencies
RUN npm install

# Copy rest of the app files
COPY . .

# Expose port used by the app
EXPOSE 3000

# Start the app
CMD ["node", "index.js"]


