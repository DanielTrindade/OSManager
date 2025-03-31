import api from './api';


class OrderService {
  getOrders() {
    return api.get('/orders');
  }

  getOrderById(id) {
    return api.get(`/orders/${id}`);
  }

  createOrder(description) {
    return api.post('/orders', { description });
  }

  updateOrderStatus(id, status, rejectionReason = null) {
    return api.put('/orders/status', { 
      id, 
      status,
      rejectionReason
    });
  }

  updateChecklistItem(id, isCompleted) {
    return api.put('/checklist/items', { id, isCompleted });
  }

  uploadImage(orderId, file, checklistItemId = null) {
    const formData = new FormData();
    formData.append('file', file);
    
    if (checklistItemId) {
      formData.append('checklistItemId', checklistItemId);
    }
    
    return api.post(`/orders/${orderId}/images`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    });
  }

  deleteImage(imageId) {
    return api.delete(`/images/${imageId}`);
  }

  getOrderStats() {
    return api.get('/orders/stats');
  }

  getFilteredOrders(status = null, fromDate = null, toDate = null) {
    let url = '/orders/filter?';
    
    if (status) {
      url += `status=${status}&`;
    }
    
    if (fromDate) {
      url += `fromDate=${fromDate.toISOString()}&`;
    }
    
    if (toDate) {
      url += `toDate=${toDate.toISOString()}&`;
    }
    
    return api.get(url);
  }
}

export default new OrderService();