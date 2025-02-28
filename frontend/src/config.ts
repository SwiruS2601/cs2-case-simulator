export const config = {
  baseUrl: import.meta.env.VITE_BASE_URL,
  apiBaseUrl: import.meta.env.VITE_API_URL,
  imageOptimizationUrl: import.meta.env.VITE_IMAGE_OPTIMIZATION_URL,
};

if (!config.apiBaseUrl) throw new Error('VITE_API_URL is not defined');
if (!config.imageOptimizationUrl) throw new Error('VITE_IMAGE_OPTIMIZATION_URL is not defined');
