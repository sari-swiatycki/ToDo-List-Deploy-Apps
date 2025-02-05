import axios from 'axios';

axios.defaults.baseURL = process.env.API_URL
axios.interceptors.response.use(
  response => response, // מחזיר את התגובה כרגיל אם אין שגיאה
  error => {
    console.error('API Error:', {
      message: error.message,
      status: error.response?.status,
      data: error.response?.data
    });
    return Promise.reject(error); // החזרת השגיאה להמשך טיפול
  }
);

export default {
  getTasks: async () => {  
      const result = await axios.get('/tasks');
      if (Array.isArray(result.data))
        return result.data
      else {
        alert("no tasks");
        return [];
      }
    
    },
  addTask: async (name) => {
    console.log('addTask', name)
    const res = await axios.post('/tasks', {name: name, isComplete:false });
    return res.data;
  },

  setCompleted: async (id, isComplete) => {
    console.log('setCompleted', { id, isComplete })
    const res = await axios.put(`/tasks/${id}`, { isComplete:isComplete });
    return res.data;
  },

  deleteTask: async (id) => {
    console.log('deleteTask');
    const res = await axios.delete(`/tasks/${id}`);
    return res.data;
  }
};






















