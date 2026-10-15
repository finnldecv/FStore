import axios from 'axios';

export const getStatusCode = (error: unknown): number | undefined => {
  if (axios.isAxiosError(error)){
    return error.response?.status;
  };
  return undefined
};

export const isNotFound = (error: unknown): boolean =>
  getStatusCode(error) === 404;