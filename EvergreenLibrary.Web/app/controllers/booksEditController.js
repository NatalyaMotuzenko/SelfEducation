'use strict';
app.controller('booksEditController', ['$scope', '$location', 'booksService', 'localStorageService', '$routeParams', function ($scope, $location, booksService, localStorageService, $routeParams) {

    $scope.editBookData = {};

    $scope.message = "";

    $scope.init = function () {
        var isLibrarian = localStorageService.get('authorizationData') !== null && localStorageService.get('authorizationData').roleName === "Librarian";
        if (!isLibrarian) {
            $location.path('/error');
        }
    };

    booksService.getBookById($routeParams.bookId).then(function (results) {

        $scope.editBookData = results.data;

    }, function (error) {
        $location.path('/error');
    });

    $scope.editBook = function () {

        booksService.editBook($routeParams.bookId, $scope.editBookData).then(editBookSuccess, editBookError);
        function editBookSuccess(response) {
            $location.path('/books');
        }
        function editBookError(err) {
            $scope.message = err.data.error_description === undefined ? err.data.message : err.data.error_description;
        }
    };

}]);