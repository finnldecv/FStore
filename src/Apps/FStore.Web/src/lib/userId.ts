const STORAGE_KEY = "fstore_user_id";

export const getUserId = (): string =>{
 let userId = localStorage.getItem(STORAGE_KEY);

 if (!userId){
  userId = crypto.randomUUID();
  localStorage.setItem(STORAGE_KEY, userId);
 }

 return userId;
}