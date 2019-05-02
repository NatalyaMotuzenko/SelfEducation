'use strict';
app.controller('booksAddController', ['$scope', '$location', 'booksService', 'localStorageService', function ($scope, $location, booksService, localStorageService) {
    $scope.addBookData = {
        title: "",
        author: "",
        year: 0
    };

    $scope.message = "";

    $scope.init = function () {
        var isLibrarian = localStorageService.get('authorizationData') !== null && localStorageService.get('authorizationData').roleName === "Librarian";
        if (!isLibrarian) {
            $location.path('/error');
        }
    };

    $scope.init();

    $scope.addBook = function () {

        booksService.addBook($scope.addBookData).then(addBookSuccess, addBookError);
        function addBookSuccess(response) {
                $location.path('/books');
        }
        function addBookError(err) {
            $scope.message = err.data.error_description === undefined ? err.data.message : err.data.error_description;
        }
    };

}]);