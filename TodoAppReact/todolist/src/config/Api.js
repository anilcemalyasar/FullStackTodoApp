// src/config/api.js - Debug version
const config = {
  development: {
    API_BASE_URL: 'http://localhost:5159/api',
  },
  docker: {
    API_BASE_URL: '/api',  // nginx proxy
  },
  production: {
    API_BASE_URL: '/api',
  }
};

const currentEnv = process.env.REACT_APP_ENV || process.env.NODE_ENV || 'development';

export const API_BASE_URL = config[currentEnv]?.API_BASE_URL || config.development.API_BASE_URL;

export default {
  API_BASE_URL,
  ENVIRONMENT: currentEnv,
  IS_DEVELOPMENT: currentEnv === 'development',
  IS_DOCKER: currentEnv === 'docker',
  IS_PRODUCTION: currentEnv === 'production'
};