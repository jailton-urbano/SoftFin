$(document).ready(function () {
    Metronic.init(); // init metronic core components
    Layout.init(); // init current layout
    QuickSidebar.init(); // init quick sidebar
});

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, "\\$&");
    var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)"),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, " "));
}

var app = angular.module('SOFTFIN', ['ui.bootstrap', 'ui.utils.masks', 'angularUtils.directives.dirPagination']);

app.directive('datetimez', function () {
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ngModelCtrl) {

            element.datetimepicker({
                dateFormat: 'dd/MM/yyyy',
                language: 'ptbr',
                pickTime: false,
                startDate: '01-11-2013',      // set a minimum date
                endDate: '01-11-2030'          // set a maximum date
            }).on('changeDate', function (e) {
                ngModelCtrl.$setViewValue(moment(e.localDate).format());
                scope.$apply();
                $(this).datetimepicker('hide');
            });
        }
    };
});


app.filter('yesNo', function () {
    return function (input) {
        return input == "True" ? 'Sim' : 'Não';
    }
})





app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', function ($scope, $http, $location, $anchorScroll, $timeout, $modal) {

        $scope.EditAccess = false;
        $http.post("../../TransfConta/AcessoEdicao")
            .success(function (data) {
                $scope.EditAccess = data;
            }).error(function () {
                Metronic.unblockUI();
                $scope.MsgError("Sistema indisponivel");
            });


        $scope.ListaNotas = [];
        $scope.idSelecionado = 0
        $scope.showMostraTodos = true;
        $scope.procurar = "";
        $scope.showCancelar = false;
        $scope.showCadastro = false;
        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.msgSalvar = "Salvar";
        $scope.btnSalvar = true;
        $scope.showGrid = true;
        $scope.showloadGrid = false;
        $scope.ModoConsulta = false;
        
        $scope.filtro = {
            banco_id: "",
            dataIni: moment().add(-8, 'days').format(),
            dataFim: moment().add(8, 'days').format()

        };

        var dataParam = getParameterByName("DataInicial");
        if (dataParam != null) {
            $scope.filtro.dataIni = dataParam;
            $scope.filtro.dataFim = dataParam;
        }



        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];

        $scope.Novo = function () {
            $scope.ModoConsulta = false;

            $scope.LoadporId(0);
        }

        $scope.Voltar = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            $scope.Pesquisa();
        }
        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.LoadporId(item.Id);
        }

        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.LoadporId(item.Id);
        }

        $scope.MsgError = function (msg) {
            $scope.msgNOK = msg;
            $scope.showNOK = true;
            $scope.showOK = false;
        }

        $scope.MsgSucesso = function (msg) {
            $scope.msgOK = msg;
            $scope.showOK = true;
            $scope.showNOK = false;
        }


        $scope.Pesquisa = function (msg) {
            $scope.LoadAll();
        }


        $scope.Excel = function (msg) {
            window.location = '../../BM/Excel';
        }

        $scope.Salvar = function () {



            $scope.opts = {
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalSalvar.html',
                controller: ControllerSalvar,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: $scope.entidade }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (data) {
                $scope.showOK = true;
                $scope.msgOK = data.DSMessage;
                $timeout(
                    function () {
                        $scope.showNOK = false;
                        $scope.showOK = false;
                        $scope.msgSalvar = "Salvar";
                        $scope.btnSalvar = true;

                    }, 18000);
            }, function () {
            });


        }

        $scope.cancel = function () {
            $scope.showImportacao = false;
            $scope.showMostraTodos = true;
            $location.hash('DivTotal');
        };


        $scope.LoadListaBanco = function () {
            $http.post("../../CG/ObterBanco")
                .success(function (data) {
                    $scope.ListaBancos = data;
                });
        };
        $scope.LoadListaBanco();



        $scope.LoadListaUnidades = function () {
            $http.post("../../CG/ObterUnidades")
                .success(function (data) {
                    $scope.ListaUnidades = data;
                });
        };
        $scope.LoadListaUnidades();



        $scope.LoadAll = function () {
            Metronic.blockUI({
                boxed: true
            });

            $http.post("../../TransfConta/ObterTodos/", $scope.filtro)
            .success(function (data) {
                $scope.lista = data;
                Metronic.unblockUI();
            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
                Metronic.unblockUI();
            });
        }
        //$scope.LoadAll();

        $scope.LoadporId = function (id) {

            $http.get("../../TransfConta/ObterPorId/" + id)
                .success(function (data) {
                    $scope.entidade = data;
                    $scope.showGrid = false;
                    $scope.showManut = true;



            }).error(function (data) {
                $scope.MsgError("Sistema indisponivel");
            });
        }

        $scope.Excluir = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalExcluir.html',
                controller: ControllerExcluir,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.LoadAll();
            }, function () {
            });
        }


    }
]);

var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, item) {



    $scope.item = item.item;

    $scope.ok = function () {
        Metronic.blockUI({
            boxed: true
        });

        $http.post("../../TransfConta/Excluir",  $scope.item )
            .success(function (data) {
                Metronic.unblockUI();
            if (data.CDStatus == "OK") {
                $modalInstance.close(data);
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;
                $scope.Erros = data.Erros;
            }
            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;

                }, 8000);
            }).error(function (data) {
                Metronic.unblockUI();
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}



var ControllerSalvar = function ($scope, $modalInstance, $modal, $http, $timeout, item) {

    $scope.entidade = item.item;
    $scope.mensagem = "";



    $scope.ok = function () {
        $http.post("../../TransfConta/Salvar", $scope.entidade )
        .success(function (data) {
            if (data.CDStatus == "OK") {
                $modalInstance.close(data);
            } else {
                $scope.showNOK = true;
                $scope.msgNOK = data.DSMessage;
                $scope.Erros = data.Erros;
            }
            $timeout(
                function () {
                    $scope.showNOK = false;
                    $scope.showOK = false;

                }, 8000);
        }).error(function (data) {
            $scope.showNOK = true;
            $scope.msgNOK = "Sistema indisponivel";
        });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}