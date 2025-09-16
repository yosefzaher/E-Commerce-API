# 🛍️ ClickToBuy

ClickToBuy is an **ECommerce RESTful API** designed to manage **products, categories, shopping carts, orders, and users**.  
It serves as the backend for an online shopping platform, making it easy to integrate with web and mobile applications.  

---

## 🛠 Tech Stack

- **Backend Framework:** ASP.NET Core Web API  
- **Language:** C#  
- **Database:** SQL Server with Entity Framework Core  
- **Architecture:** Layered / 3-Tier Architecture  
- **Testing & Docs:** Swagger, Postman  

---

## 🚀 Features

### Core Features:
- **Product Management** – Add, update, delete, and search products.  
- **Category Management** – Organize and manage product categories.  
- **Shopping Cart** – Add, remove, and view cart items.  
- **Order Management** – Place, track, and delete user orders.  
- **User Management** – Manage registered users.  
- **Search** – Filter products quickly and easily.  

---

## 🎨 Frontend

- React  
- Tailwind CSS  
- HTML  
- CSS  
- JavaScript (integrated with the API)  

---

## ⚙️ Backend

- **Server Framework:** ASP.NET Core Web API  
- **ORM:** Entity Framework Core  
- **Database:** SQL Server  
- **Architecture:** 3-Tier (Controller → Service → Repository)  

---

## 📡 API Endpoints

### 🗂 Categories
- `GET /api/Categories/GetAllCategories`  
- `GET /api/Categories/{Id}`  
- `POST /api/Categories/AddCategory`  
- `PUT /api/Categories/{Id}`  
- `DELETE /api/Categories/{Id}`  

---

### 🛒 Products
- `GET /api/Products/GetAllProucts`  
- `GET /api/Products/{Id}`  
- `POST /api/Products/AddProdcut`  
- `PUT /api/Products/{Id}`  
- `DELETE /api/Products/{Id}`  
- `GET /api/Products/Search`  

---

### 📦 Orders
- `POST /api/Orders/PlaceOrder/{Id}`  
- `GET /api/Orders/GetProductsDetails/{Id}`  
- `GET /api/Orders/GetUserOrders/{Id}`  
- `DELETE /api/Orders/DeleteUserOrder/{Id}`  
- `DELETE /api/Orders/ClearUserOrders/{Id}`  

---

### 🛍 Shopping Carts
- `POST /api/ShoppingCarts/AddtoShoopingCart`  
- `GET /api/ShoppingCarts/GetAllShoppingCart`  
- `DELETE /api/ShoppingCarts/RemoveFromCart/{Id}`  
- `PATCH /api/ShoppingCarts/ChangeCartQuantity/{Id}`  

---

### 👤 Users
- `GET /api/Users`  

---

### ❤️ Wishlists
- `GET /api/Wishlists/GetUserWishlist/{userId}`  
- `POST /api/Wishlists/AddToUserWishlist/{userId}/{productId}`  
- `DELETE /api/Wishlists/RemoveFromUserWishlist/{userId}/{productId}`  

---

### 🔑 Authentication
- `POST /api/Auth/Register`  
- `POST /api/Auth/Login`  
- `POST /api/Auth/RefreshToken`  
- `PUT /api/Auth/Revoke-RefreshToken`  
---

## 🖥️ User Inteface
![WhatsApp Image 2025-09-14 at 9 50 27 PM](https://github.com/user-attachments/assets/7e79daec-c990-4525-92cd-ba5801028c39)


![06b59811-8fa2-4efa-8f45-7796bc152fa2](https://github.com/user-attachments/assets/7f843f3c-0d5f-49c7-b0c8-8ded8a230eae)

