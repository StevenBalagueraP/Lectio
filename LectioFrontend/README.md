## Frontend Structure
```
src/
├── api/
│   ├── client.ts         
│   ├── endpoints/
│   │   ├── auth.api.ts        
│   │   ├── users.api.ts
│   │   └── products.api.ts
│   └── types/
│       ├── auth.types.ts
│       └── products.types.ts
│
├── features/
│   ├── auth/
│   │   ├── components/
│   │   │   ├── LoginForm.tsx
│   │   │   └── RegisterForm.tsx
│   │   ├── hooks/
│   │   │   └── useAuth.ts
│   │   ├── context/
│   │   │   └── AuthContext.tsx
│   │   └── pages/
│   │       ├── LoginPage.tsx
│   │       └── RegisterPage.tsx
│   │
│   ├── dashboard/
│   │   ├── components/
│   │   ├── hooks/
│   │   └── pages/
│   │       └── DashboardPage.tsx
│   │
│   └── products/
│       ├── components/
│       ├── hooks/
│       │   └── useProducts.ts  
│       └── pages/
│           └── ProductsPage.tsx
│
├── routes/
│   ├── AppRouter.tsx          
│   ├── ProtectedRoute.tsx    
│   └── PublicRoute.tsx       
│
├── shared/
│   ├── components/
│   │   ├── ui/               
│   │   └── layout/
│   │       ├── Navbar.tsx
│   │       └── Sidebar.tsx
│   ├── hooks/
│   │   └── useLocalStorage.ts
│   └── utils/
│       └── formatters.ts
│
├── config/
│   └── env.ts                 
│
├── App.tsx
├── main.tsx
└── index.css
```