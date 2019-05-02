'use strict';
app.controller('booksController', ['$scope', 'booksService', 'userBooksService', 'localStorageService', '$location', '$route', function ($scope, booksService, userBooksService, localStorageService, $location, $route) {

    $scope.books = [];

    $scope.userName = localStorageService.get('authorizationData')? localStorageService.get('authorizationData').userName : "";

    $scope.message = "";

    booksService.getBooks().then(function (results) {

        $scope.books = results.data;

    }, function (error) {
        $scope.message = error.data.message;
        });

    $scope.deleteBook = function (id) {
        var bookId = $scope.books[id].id;

        booksService.deleteBook(bookId).then(deleteBookSuccess, deleteBookError);
        function deleteBookSuccess(response) {
            $scope.books.splice(id, 1);
        }
        function deleteBookError(err) {
            $scope.message = err.data.error_description === undefined ? err.data.message : err.data.error_description;
        }
    };

    $scope.readBook = function (id) {
        var bookId = $scope.books[id].id;

        userBooksService.readTakeBook(bookId, true).then(readTakeBookSuccess, readTakeBookError);
        function readTakeBookSuccess(response) {
            $scope.books[id].state = $scope.userName;
        }
        function readTakeBookError(err) {
            $scope.message = err.data.error_description === undefined ? err.data.message : err.data.error_description;
        }
    };

    $scope.returnBook = function (id) {
        var bookId = $scope.books[id].id;

        userBooksService.readTakeBook(bookId, false).then(readTakeBookSuccess, readTakeBookError);
        function readTakeBookSuccess(response) {
            $scope.books[id].state = "free";
        }
        function readTakeBookError(err) {
            $scope.message = err.data.error_description === undefined ? err.data.message : err.data.error_description;
        }
    };

}]);