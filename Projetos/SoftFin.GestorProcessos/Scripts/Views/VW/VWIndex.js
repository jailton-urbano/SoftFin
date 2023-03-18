var app = angular.module('SOFTFIN', ['ui.bootstrap', 'ui.utils.masks', 'angularUtils.directives.dirPagination', 'blockUI']);
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
        return input == true ? 'Sim' : 'Não';
    }
})

app.controller('MestreEntidade', [
    '$scope', '$http', '$location', '$anchorScroll', '$timeout', '$modal', 'blockUI','$sce',
    function ($scope, $http, $location, $anchorScroll, $timeout, $modal, blockUI, $sce) {
        blockUI.start("Aguarde...");
        //Variaveis
        $scope.procurar = "";
        $scope.showGrid = true;
        $scope.showManut = false;
        $scope.showOK = false;
        $scope.showNOK = false;
        $scope.msgOK = "";
        $scope.msgNOK = "";
        $scope.reverse = false;
        $scope.ModoConsulta = false;
        $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];

        //Funcões Staticas
        $scope.MsgError = function (msg) {
            $scope.msgNOK = msg;
            $scope.showNOK = true;
            $scope.showOK = false;
        }
        $scope.ordenar = function (keyname) {
            $scope.sortKey = keyname;
            $scope.reverse = !$scope.reverse;
        };
        //Funcões Botôes
        $scope.ObterTodos = function () {
            blockUI.start("Aguarde...");

            $http.post("../../VW/ObterTodos/")
                .success(function (data) {
                    blockUI.stop();
                    $scope.lista = data;
                    
                }).error(function (data) {
                    blockUI.stop();
                    $scope.MsgError("Sistema indisponivel");
                    
                });
        }
        $scope.ObterTodosCampos = function () {
            blockUI.start("Aguarde...");

            $http.post("../../VW/ObterTodosCampos/", { idVisao: $scope.visao.Id } )
                .success(function (data) {
                    blockUI.stop();
                    $scope.visao.VisaoCampos = data;

                }).error(function (data) {
                    blockUI.stop();
                    $scope.MsgError("Sistema indisponivel");

                });
        }

        $scope.Novo = function () {
            $scope.ModoConsulta = false;
            $scope.ObterPorId(0);
        }
        $scope.Voltar = function () {
            $scope.showGrid = true;
            $scope.showManut = false;
            $scope.ModoConsulta = false;
        }

        $scope.Alterar = function (item) {
            $scope.ModoConsulta = false;
            $scope.ObterPorId(item.Id);
        }

        $scope.Detalhar = function (item) {
            $scope.ModoConsulta = true;
            $scope.ObterPorId(item.Id);
        }

        $scope.Salvar = function () {
            blockUI.start("Aguarde...");

            $http.post("../../VW/Salvar", $scope.visao)
                .success(function (data) {
                    blockUI.stop();

                    if (data.CDStatus == "OK") {
                        $scope.showOK = true;
                        $scope.msgOK = data.DSMessage;
                        
                    } else {
                        $scope.showNOK = true;
                        $scope.Erros = data.Erros;
                        $scope.msgNOK = data.DSMessage;
                        
                    }

                    $timeout(
                        function () {
                            $scope.showNOK = false;
                            $scope.showOK = false;
                        }, 8000);



                }).error(function (data) {
                    blockUI.stop();
                    $scope.showNOK = true;
                    $scope.msgNOK = "Sistema indisponivel";
                });
        }

        $scope.ObterPorId = function (value) {
            blockUI.start("Aguarde...");

            $http.post("../../VW/ObterPorId", { id: value })
                .success(function (data) {
                    
                    $scope.showGrid = false;
                    $scope.showManut = true;
                    blockUI.stop();
                    $scope.visao = data;
                    $scope.MostraIframe();
                }).error(function (data) {
                    blockUI.stop();
                    $scope.MsgError("Sistema indisponivel");
                });
        }

        $scope.LoadTabelas = function () {
            $http.post("../../VW/ObterTabela")
                .success(function (data) {
                    $scope.ListaTabela = data;
                });
        };

        $scope.LoadVisao = function () {
            $http.post("../../VW/ObterTipoVisao")
                .success(function (data) {
                    $scope.ListaVisao = data;
                });
        };
        $scope.showiframe = false;
        $scope.MostraIframe = function () {
            $scope.showiframe = true;
            $scope.urliframe = $sce.trustAsResourceUrl("../../Dinamico/ShowTable?visao=" + $scope.visao.Id);
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
                $scope.ObterTodos();
            }, function () {
            });
        }

        $scope.NovoCampo = function () {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalNovo.html',
                controller: ControllerNovoCampo,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({
                    item: {
                        "Id": 0,
                        "Ativo": true,
                        "IdVisao": $scope.visao.Id,
                        "IdTabela": $scope.visao.IdTabela
                    }
                }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.ObterTodosCampos();
            }, function () {
            });
        }

        $scope.EditarCampo = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalNovo.html',
                controller: ControllerEditarCampo,
                resolve: {} // empty storage
            };
            item.IdTabela = $scope.visao.IdTabela;
            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.ObterTodosCampos();
            }, function () {
            });
        }
        $scope.ExcluirCampo = function (item) {
            $scope.opts = {
                size: 'lg',
                animation: true,
                backdrop: false,
                backdropClick: false,
                dialogFade: false,
                keyboard: true,
                templateUrl: 'modalExcluirCampo.html',
                controller: ControllerExcluirCampo,
                resolve: {} // empty storage
            };

            $scope.opts.resolve.item = function () {
                return angular.copy({ item: item }); // pass name to Dialog
            }

            var modalInstance = $modal.open($scope.opts);

            modalInstance.result.then(function (value) {
                $scope.ObterTodosCampos();
            }, function () {
            });
        }
        $scope.ObterTodos();
        $scope.LoadTabelas();
        $scope.LoadVisao();
        $timeout(
            function () {
                blockUI.stop();

            }, 1000);

    }]);




var ControllerExcluir = function ($scope, $modalInstance, $modal, $http, $timeout, blockUI, item) {



    $scope.item = item.item;

    $scope.ok = function () {
        blockUI.start("Aguarde...");
        $http.post("../../VW/Excluir", { obj: $scope.item })
            .success(function (data) {
                blockUI.stop();
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
                blockUI.stop();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}

var ControllerExcluirCampo = function ($scope, $modalInstance, $modal, $http, $timeout, blockUI, item) {

    $scope.item = item.item;

    $scope.ok = function () {
        blockUI.start("Aguarde...");
        $http.post("../../VW/ExcluirCampo", { obj: $scope.item })
            .success(function (data) {
                blockUI.stop();
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
                blockUI.stop();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}

var ControllerEditarCampo = function ($scope, $modalInstance, $modal, $http, $timeout, blockUI,item) {

    $scope.item = item.item;
    $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
    $scope.LoadObterTabelaCampos = function () {
        $http.post("../../VW/ObterTabelaCampos", { tabela_id: $scope.item.IdTabela })
            .success(function (data) {
                $scope.ListaCampos = data;
            });
    };
    $scope.LoadObterTabelaCampos();


    $scope.ok = function () {
        blockUI.start("Aguarde...");
        $http.post("../../VW/SalvarCampo", { obj: $scope.item })
            .success(function (data) {
                blockUI.stop();
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
                blockUI.stop();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}

var ControllerNovoCampo = function ($scope, $modalInstance, $modal, $http, $timeout, blockUI,item) {

    $scope.item = item.item;
    $scope.ListaSimNao = [{ Value: false, Text: "Não" }, { Value: true, Text: "Sim" }];
    $scope.LoadObterTabelaCampos = function () {
        $http.post("../../VW/ObterTabelaCampos", { tabela_id: $scope.item.IdTabela })
            .success(function (data) {
                $scope.ListaCampos = data;
            });
    };
    $scope.LoadObterTabelaCampos();



    $scope.ok = function () {
        blockUI.start("Aguarde...");
        $http.post("../../VW/SalvarCampo", { obj: $scope.item })
            .success(function (data) {
                blockUI.stop();
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
                blockUI.stop();
                $scope.showNOK = true;
                $scope.msgNOK = "Sistema indisponivel";
            });
    }

    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    };
}
