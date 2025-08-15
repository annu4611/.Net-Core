# E-Commerce Web Application

A fully functional e-commerce web application built with .NET MVC, Entity Framework, and SQLite database.

## Features

### 🔐 User Authentication and Authorization
- User registration and login functionality
- Role-based access control (Admin, Seller, Buyer)
- Profile management capabilities
- Secure password requirements

### 📦 Product Management
- Complete CRUD operations for products
- Product attributes: name, description, price, category, stock quantity
- Multiple image upload support for products
- Category-based product organization
- Brand and SKU tracking

### 🛍️ Product Catalogue and Search
- Product listing with pagination (10 products per page)
- Advanced search functionality
- Filter by categories, price range, and minimum ratings
- Detailed product pages with comprehensive product information
- Responsive product grid layout

### ⭐ Reviews and Ratings System
- Interactive 5-star rating system for products
- User reviews with comments and ratings
- Average rating calculation and display
- Rating-based product filtering
- Hollow gray stars that fill with golden color on selection
- Hover effects and smooth animations
- One review per user per product policy
- Review management (users can delete their own reviews)

### 👥 User Roles
- **Admin**: Full system access, can manage all products, users, and reviews
- **Seller**: Can create, edit, and delete their own products; can view all reviews
- **Buyer**: Can browse products, view details, write reviews, and rate products

## Technology Stack

- **Framework**: ASP.NET Core MVC (.NET 9.0)
- **Database**: SQLite (embedded database)
- **ORM**: Entity Framework Core
- **Authentication**: ASP.NET Core Identity
- **Frontend**: Bootstrap 5, Font Awesome icons
- **Development Environment**: Visual Studio 2019/2022

## Prerequisites

- Visual Studio 2019 or 2022
- .NET 9.0 SDK
- Git (for version control)

## Installation and Setup

### 1. Clone the Repository
```bash
git clone [your-repository-url]
cd E-Commerce-Website
```

### 2. Navigate to Project Directory
```bash
cd ECommerceWebApp
```

### 3. Restore NuGet Packages
```bash
dotnet restore
```

### 4. Build the Project
```bash
dotnet build
```

### 5. Run the Application
```bash
dotnet run
```

The application will be available at `https://localhost:5001` or `http://localhost:5000`

## Database

The application uses SQLite as an embedded database, which means:
- No separate database server installation required
- Database file (`ecommerce.db`) is created automatically in the project root
- All data is stored locally in the SQLite file
- Perfect for development and demonstration purposes

### Database Initialization
The application automatically:
- Creates the database and tables on first run
- Seeds initial categories (Electronics, Clothing, Books, Home & Garden, Sports)
- Creates default user accounts with different roles
- Sets up the Reviews table for product ratings and feedback

## Default User Accounts

The application comes with pre-configured demo accounts:

| Role | Email | Password | Description |
|------|-------|----------|-------------|
| Admin | admin@ecommerce.com | Admin123! | Full system access |
| Seller | seller@ecommerce.com | Seller123! | Can manage products |
| Buyer | buyer@ecommerce.com | Buyer123! | Can browse and review |

## Project Structure

```
ECommerceWebApp/
├── Controllers/          # MVC Controllers
│   ├── AccountController.cs
│   ├── HomeController.cs
│   ├── ProductsController.cs
│   ├── ProfileController.cs
│   └── ReviewsController.cs
├── Data/                # Database Context
│   └── ApplicationDbContext.cs
├── Models/              # Entity Models
│   ├── User.cs
│   ├── Product.cs
│   ├── Category.cs
│   ├── ProductImage.cs
│   ├── Review.cs
│   └── ErrorViewModel.cs
├── ViewModels/          # View Models
├── Views/               # Razor Views
│   ├── Account/
│   ├── Home/
│   ├── Products/
│   └── Shared/
├── Services/            # Business Logic
│   └── DbInitializer.cs
└── wwwroot/            # Static Files
    ├── css/
    ├── js/
    └── images/
```

## Key Features Demonstration

### 1. User Registration and Login
- Navigate to `/Account/Register` to create a new account
- Use `/Account/Login` to sign in with existing credentials
- New users are automatically assigned the "Buyer" role

### 2. Product Management (Admin/Seller)
- Add new products with multiple images
- Edit existing product details
- Manage product categories and inventory
- Soft delete products (mark as inactive)

### 3. Product Browsing and Reviews
- Browse all products with pagination (10 products per page)
- Search products by name or description
- Filter by category, price range, and minimum rating
- View detailed product information with reviews
- Interactive star rating system for writing reviews
- View average ratings and review counts

### 4. Responsive Design
- Mobile-friendly interface
- Bootstrap-based responsive layout
- Touch-friendly navigation and controls

## Development Notes

### Database Migrations
The application uses Entity Framework Code First approach:
- Models define the database schema
- Database is created automatically using `EnsureCreated()`
- For production, consider using proper migrations

### Image Upload
- Product images are stored in `wwwroot/images/products/`
- Supports multiple image formats (JPG, PNG, GIF)
- Images are renamed with GUIDs to prevent conflicts

### Security Features
- Password requirements enforced
- Role-based authorization on controllers
- Anti-forgery tokens on forms
- Input validation and sanitization

## Recent Updates

### ⭐ Reviews and Ratings System (Latest)
- **Interactive Star Rating**: Hollow gray stars that fill with golden color
- **User Reviews**: Customers can write detailed reviews with ratings
- **Rating Filters**: Filter products by minimum rating (1-5 stars)
- **Average Ratings**: Automatic calculation and display of product ratings
- **Review Management**: Users can delete their own reviews, admins can manage all
- **Visual Enhancements**: Smooth animations and hover effects
- **One Review Policy**: Each user can review a product only once

## Future Enhancements

- Shopping cart functionality
- Order processing and payment integration
- Email notifications
- Inventory management
- Sales reporting and analytics
- Multi-language support
- Wishlist functionality
- Product comparison features

## Troubleshooting

### Common Issues

1. **Proxy/Network Issues (Corporate Environment)**
   - **Error**: `NU1301: Unable to load the service index for source https://api.nuget.org/v3/index.json`
   - **Solution**: See `PROXY_TROUBLESHOOTING.md` for detailed solutions
   - **Quick Fix**: Run `configure-proxy.ps1` as Administrator with your credentials
   - **Alternative**: Contact IT for internal NuGet feed configuration

2. **Package Restore Issues**
   - Run `dotnet restore` in the project directory
   - Try: `dotnet restore --no-http-cache --disable-parallel`
   - Clear NuGet cache: `dotnet nuget locals all --clear`
   - If behind proxy, configure proxy settings (see PROXY_TROUBLESHOOTING.md)

3. **Database Connection Issues**
   - Ensure the application has write permissions in the project directory
   - Check if `ecommerce.db` file is created in the project root

4. **Build Errors**
   - Ensure .NET 8.0 SDK is installed
   - Check for missing dependencies
   - Try building without restore: `dotnet build --no-restore`

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is for educational and demonstration purposes.

## Contact

For questions or support, please create an issue in the repository.
