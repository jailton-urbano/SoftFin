/** Depoimentos **/
jQuery('#CarDepoimentos').owlCarousel({
    loop:true,
    margin:0,
    nav:true,
    navText: ['<i class="fa fa-chevron-left"></i>','<i class="fa fa-chevron-right"></i>'],
    responsive:{
        0:{
            items:1
        },
        600:{
            items:1
        },
        1000:{
            items:1
        }
    }
});

jQuery('#Carbanner').owlCarousel({
    loop:true,
    margin:0,
    autoplay:true,
    autoPlaySpeed: 3000,
    autoPlayTimeout: 3000,
    autoplayHoverPause: false,
    nav:true,
    navText: ['<i class="fa fa-chevron-left favicon_banner1"></i>','<i class="fa fa-chevron-right favicon_banner2"></i>'],
    responsive:{
        0:{
            items:1
        },
        600:{
            items:1
        },
        1000:{
            items:1
        }
    }
});

jQuery('#Carsoftware').owlCarousel({
    loop:true,
    margin:0,
    nav:false,
    navText: ['<i class="fa fa-chevron-left"></i>','<i class="fa fa-chevron-right"></i>'],
    responsive:{
        0:{
            items:1
        },
        600:{
            items:1
        },
        1000:{
            items:1
        }
    }
});

jQuery('#monitorCarouselmobile').owlCarousel({
    loop:true,
    margin:0,
    nav:false,
    responsive:{
        0:{
            items:1
        },
        600:{
            items:1
        },
        1000:{
            items:1
        }
    }
});


jQuery('.funcionalidadesCarousel').owlCarousel({
    loop:true,
    margin:10,
    Dots:true,
    nav:true,
    navText: ['<i class="fa fa-chevron-left style_left_funcio"></i>','<i class="fa fa-chevron-right style_right_funcio"></i>'],
    responsive:{
        0:{
            items:1
        },
        600:{
            items:1
        },
        1000:{
            items:1
        }
    }
});


jQuery(function(){   
			var nav = jQuery('.menuHeader');   
			jQuery(window).scroll(function () { 
				if (jQuery(this).scrollTop() > 100) { 
					nav.addClass("menu-fixo"); 
				} else { 
					nav.removeClass("menu-fixo"); 
				} 
			});  
});

/** ICones depoimentos **/
jQuery('.botoes-change-1 .btn-change:first-of-type').addClass("svg-ativo");

jQuery('.botoes-change .btn-change').click(function(){
    jQuery('.botoes-change .btn-change').removeClass("svg-ativo");
    jQuery(this).addClass("svg-ativo");
})


/*** Monitor ***/
jQuery('#monitorCarousel').owlCarousel({
    items:1,
    loop:false,
    center:true,
    margin:0,
    URLhashListener:true,
    autoplayHoverPause:true,
    startPosition: 'URLHash'
});


//jQuery("#btn_tabela_on").click(function(){
//    setTimeout(function() {
//    jQuery(".tabela_calculadora").addClass("tabela_block");
//    }, 2000);
//});





    jQuery(document).ready(function() {
  jQuery(".carousel").carousel({
    interval: 5000
  });
  jQuery('#myCarousel').on('slide.bs.carousel', function() {


    jQuery(".myCarousel-target.active").removeClass("active");

    jQuery('#myCarousel').on('slid.bs.carousel', function() {

      var to_slide = jQuery(".carousel-item.active").attr("data-slide-no");

      jQuery(".nav-indicators li[data-slide-to=" + to_slide + "]").addClass("active");

    });
  });

});


//jQuery(document).ready(function () {
//  jQuery('div.top').click(function() {
//      setTimeout(function() {
//        jQuery('html, body').animate({
//    scrollTop: jQuery("div.table_calculadora").offset().top
//  }, 1000)  
          
//      }, 2000);
  
//})
//});