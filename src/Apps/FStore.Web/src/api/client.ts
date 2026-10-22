import axios from 'axios';

export const http = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  headers: {
    'Content-Type': 'application/json',
  },
});

http.interceptors.response.use(
  (response) => response,
  (error) => { 
    if (error.response?.status === 404) {
      return Promise.reject(error);
    }

    const message =
      error.response?.data?.error ||
      error.response?.data?.title ||
      error.message ||
      'Something went wrong';

    console.error('[API Error]', {
      url: error.config?.url,
      method: error.config?.method,
      status: error.response?.status,
      message,
    });

    return Promise.reject(error);
  }
);