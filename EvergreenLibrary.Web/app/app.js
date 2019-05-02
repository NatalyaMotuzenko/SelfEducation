var app = angular.module('AngularAuthApp', ['ngRoute', 'LocalStorageModule', 'angular-loading-bar']);

app.config(function ($routeProvider) {

    $routeProvider.when("/home", {
        controller: "homeController",
        templateUrl: "/app/views/home.html"
    });

    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "/app/views/login.html"
    });

    $routeProvider.when("/signup", {
        controller: "signupController",
        templateUrl: "/app/views/signup.html"
    });

    $routeProvider.when("/books", {
        controller: "booksController",
        templateUrl: "/app/views/books.html"
    });

    $routeProvider.when("/books/add", {
        controller: "booksAddController",
        templateUrl: "/app/views/booksAdd.html"
    });

    $routeProvider.when('/books/:bookId/edit', {
        controller: "booksEditController",
        templateUrl: "/app/views/booksEdit.html"
    });

    $routeProvider.when('/user/books', {
        template: "<user-books></user-books>"
    });

    $routeProvider.when('/accounts', {
        controller: "usersController",
        templateUrl: "/app/views/users.html"
    });

    $routeProvider.when('/accounts/add', {
        controller: "usersAddController",
        templateUrl: "/app/views/signup.html"
    });

    $routeProvider.when('/accounts/users/:userId/edit', {
        controller: "usersEditController",
        templateUrl: "/app/views/userEdit.html"
    });

    $routeProvider.when("/error", {
        templateUrl: "/app/views/error.html"
    });

    $routeProvider.otherwise({ redirectTo: "/home" });
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('authInterceptorService');
});

app.run(['authService', function (authService) {
    authService.fillAuthData();
}]);