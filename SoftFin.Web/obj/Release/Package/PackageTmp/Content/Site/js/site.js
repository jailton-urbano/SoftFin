$body = $("body");

function isEmail(email) {
    var regex = /^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/;
    return regex.test(email);
}

function MensagemSucesso() {
    $('.input').val("");
    $(".msg_sucesso").html("");
    $(".msg_sucesso").fadeIn().html('Obrigado! entraremos em contato!');
    setTimeout(function () {
        $(".msg_sucesso").html("");
        $('.msg_sucesso').fadeOut("slow");
    }, 4000);
}

function MensagemErro() {
    $(".msg_erro").show().html("Desculpe, não consegui completar sua requisição, tente novamente");
    setTimeout(function () {
        $('.msg_erro').fadeOut("slow");
    }, 4000);
}
function EnviaEmail(email) {
    $.ajax(
        {
            url: "../../Site/LeadEmail",
            data: {

                'email': email
            },
            dataType: "json",
            type: 'POST',
            success: function (data) {
                DesTravaTela()
                MensagemSucesso();
            },
            error: function () {
                DesTravaTela()
                MensagemErro();

            }
        }
    );

}
$("#btnemail01").click(function (event) {
    
    var email = $("#email01").val();

    $(".msg_erro").hide().html("");
    if (email == "") {
        $(".msg_erro").show().html("Informe o email");
        return;
    }

    if (isEmail(email) == false) {
        $(".msg_erro").show().html("Informe o email corretamente");
        return;
    }
    TravaTela();
    EnviaEmail(email);
    event.preventDefault();
});
$("#btnemail02").click(function (event) {
    
    var email = $("#email02").val();
    $(".msg_erro").hide().html("");
    if (email == "") {
        $(".msg_erro").show().html("Informe o email");
        return;
    }

    if (isEmail(email) == false) {
        $(".msg_erro").show().html("Informe o email corretamente");
        return;
    }

    TravaTela();
    EnviaEmail(email);
    event.preventDefault();
});
$("#contbtnEnviar").click(function (event) {
    event.preventDefault();
    $(".msg_erro").hide().html("");

    msg = "Informe todos os campos";

    if (isEmail($("#contemail").val()) == false) {
        $(".msg_erro").show().html("Informe o email corretamente");
        return;
    }

    if ($("#contnome").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#contemail").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#conttelefone").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#contassunto").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#contmensagem").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#contempresa").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }

    TravaTela();

    $.ajax(
        {
            url: "../../Site/LeadContato",
            data: {
                'nome': $("#contnome").val(),
                'email': $("#contemail").val(),
                'telefone': $("#conttelefone").val(),
                'assunto': $("#contassunto").val(),
                'mensagem': $("#contmensagem").val(),
                'empresa': $("#contempresa").val()
            },
            dataType: "json",
            type: 'POST',
            success: function (data) {
                DesTravaTela();
                MensagemSucesso();
            },
            error: function () {
                DesTravaTela();
                MensagemErro();
            }
        }
    );
    
});
$("#expbtnExperimente").click(function (event) {
    event.preventDefault();
    $(".msg_erro").hide().html("");

    msg = "Informe todos os campos";

    if (isEmail($("#expemail").val()) == false) {
        $(".msg_erro").show().html("Informe o email corretamente");
        return;
    }

    if ($("#expnome").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }

    if ($("#expemail").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#exptelefone").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#expsenha").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#exptemempresa").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }

    TravaTela();

    $.ajax(
        {
            url: "../../Site/LeadExperimente",
            data: {
                'nome': $("#expnome").val(),
                'email': $("#expemail").val(),
                'telefone': $("#exptelefone").val(),
                'senha': $("#expsenha").val(),
                'temempresa': $("#exptemempresa").val()
            },
            dataType: "json",
            type: 'POST',
            success: function (data) {
                DesTravaTela();
                MensagemSucesso();
            },
            error: function () {
                DesTravaTela();
                MensagemErro();
            }
        }
    );
    event.preventDefault();
});

$("#simbtnContrate").click(function (event) {
    
    event.preventDefault();
    $(".msg_erro").hide().html("");
    TravaTela();
    $.ajax(
        {
            url: "../../Site/EmailPedido",
            data: {
                
                'email': $("#simemail").val()
            },
            dataType: "json",
            type: 'POST',
            success: function (data) {
                DesTravaTela();
                $("#mensagemFinal").show();
                $("#simbtnContrate").hide();
            },
            error: function () {
                DesTravaTela();
                MensagemErro();
            }
        }
    );
    event.preventDefault();

});

$("#simbtnA").click(function (event) {
    
    $("#mensagemFinal").hide();
    event.preventDefault();
    $(".msg_erro").hide().html("");

    msg = "Informe todos os campos";

    if (isEmail($("#simemail").val()) == false) {
        $(".msg_erro").show().html("Informe o email corretamente");
        return;
    }

    if ($("#simnome").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#simemail").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }

    if ($("#simtelefone").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }

    TravaTela();
    $.ajax(
        {
            url: "../../Site/LeadExperimente",
            data: {
                'nome': $("#simnome").val(),
                'email': $("#simemail").val(),
                'telefone': $("#simtelefone").val(),
                'senha': "",
                'temempresa': "Simulação"
            },
            dataType: "json",
            type: 'POST',
            success: function (data) {
                DesTravaTela();
                flip();;
            },
            error: function () {
                DesTravaTela();
                MensagemErro();
            }
        }
    );
    event.preventDefault();
    
});
$("#frm2Enviar").click(function (event) {

    event.preventDefault();
    $(".msg_erro").hide().html("");

    msg = "Informe todos os campos";

    if (isEmail($("#frm2Email").val()) == false) {
        $(".msg_erro").show().html("Informe o email corretamente");
        return;
    }

    if ($("#frm2Nome").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#frm2Email").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#frm2Telefone").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#frm2Empresa").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    TravaTela();

    $.ajax(
        {
            url: "../../Site/LeadPrincipal",
            data: {
                'nome': $("#frm2Nome").val(),
                'email': $("#frm2Email").val(),
                'telefone': $("#frm2Telefone").val(),
                'empresa': $("#frm2Empresa").val()
            },
            dataType: "json",
            type: 'POST',
            success: function (data) {
                DesTravaTela();
                MensagemSucesso();
            },
            error: function () {
                DesTravaTela();
                MensagemErro();

            }
        }
    );
    event.preventDefault();

});
$("#frmEnviar").click(function (event) {

    event.preventDefault();
    $(".msg_erro").hide().html("");

    msg = "Informe todos os campos";

    if (isEmail($("#frmEmail").val()) == false) {
        $(".msg_erro").show().html("Informe o email corretamente");
        return;
    }

    if ($("#frmNome").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#frmTelefone").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#frmEmpresa").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    TravaTela();

    $.ajax(
        {
            url: "../../Site/LeadPrincipal",
            data: {
                'nome': $("#frmNome").val(),
                'email': $("#frmEmail").val(),
                'telefone': $("#frmTelefone").val(),
                'empresa': $("#frmEmpresa").val()
            },
            dataType: "json",
            type: 'POST',
            success: function (data) {
                DesTravaTela()
                MensagemSucesso();
            },
            error: function () {
                DesTravaTela()
                MensagemErro();

            }
        }
    );
    

});
$('.somentenumerico').keyup(function () {
    $(this).val(this.value.replace(/\D/g, ''));
});
$("#btn_tabela_on").click(function (event) {

    event.preventDefault();
    $(".msg_erro").hide().html("");

    msg = "Informe todos os campos";


    if ($("#calcempresas").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#calcestabelecimentos").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }

    if ($("#calcbancomes").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }

    if ($("#calccpagmes").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#calcnfsmes").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    if ($("#calcnfemes").val() == "") {
        $(".msg_erro").show().html(msg);
        return;
    }
    TravaTela();

    $.ajax(
        {
            url: "../../Site/CalcularNota",
            data: {
                'empresas': $("#calcempresas").val(),
                'estabelecimentos': $("#calcestabelecimentos").val(),
                'bancomes': $("#calcbancomes").val(),
                'cpagmes': $("#calccpagmes").val(),
                'nfsmes': $("#calcnfsmes").val(),
                'nfemes': $("#calcnfemes").val()
            },
            dataType: "json",
            type: 'POST',
            success: function (data) {
                DesTravaTela();
                
                $(".tabela_calculadora").addClass("tabela_block");
                $("#cotacao").html(data.orcamento);
                $("#cotacaoTotal").html(data.orcamentoTotal);
                $("#assistente").html(data.ladoA.Assistente);
                $("#refeicao").html(data.ladoA.Refeicao);
                $("#transporte").html(data.ladoA.Transporte);
                $("#custos").html(data.ladoA.Custos);
                $("#total").html(data.ladoA.Total);



                $('html, body').animate({
                    scrollTop: $(".title_etapa2").offset().top + 300
                }, 500);
            },
            error: function () {
                DesTravaTela();
                MensagemErro();
            }
        }
    );
    event.preventDefault();

});
function TravaTela() {
    $(".simple-msg").html("<div class='bloco_img_gif_form'><img src='http://www.letsdofind.com/business/images/loader.gif' class='loading_gif' style='display:block'/></div>");
}
function DesTravaTela() {
    $(".simple-msg").html("");
}