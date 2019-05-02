'use strict';
app.factory('booksService', ['$http', function ($http) {

    var serviceBase = 'http://localhost:59822/';
    var booksServiceFactory = {};

    var _getBooks = function () {

        return $http.get(serviceBase + 'api/books').then(function (result) {
            return result;
        });
    };

    var _addBook = function (book) {

        return $http.post(serviceBase + 'api/books', book).then(function (responce) {
            return responce;
        });
    };

    var _editBook = function (bookId, book) {

        return $http.put(serviceBase + 'api/books/' + bookId, book).then(function (responce) {
            return responce;
        });
    };

    var _getBookById = function (bookId) {

        return $http.get(serviceBase + 'api/books/' + bookId).then(function (result) {
            return result;
        });
    };

    var _deleteBook = function (bookId) {

        return $http.delete(serviceBase + 'api/books/' + bookId).then(function (responce) {
            return responce;
        });
    };

    booksServiceFactory.getBooks = _getBooks;
    booksServiceFactory.addBook = _addBook;
    booksServiceFactory.editBook = _editBook;
    booksServiceFactory.getBookById = _getBookById;
    booksServiceFactory.deleteBook = _deleteBook;

    return booksServiceFactory;

}]);