/* JS Document */

/******************************

[Table of Contents]

1. Vars and Inits
2. Set Header
3. Init Menu
4. Init Favorite
5. Init Fix Product Border
6. Init Isotope Filtering
7. Init Price Slider
8. Init Checkboxes



******************************/

jQuery(document).ready(function($)
{
	"use strict";

	/* 

	1. Vars and Inits

	*/

	var header = $('.header');
	var topNav = $('.top_nav')
	var mainSlider = $('.main_slider');
	var hamburger = $('.hamburger_container');
	var menu = $('.hamburger_menu');
	var menuActive = false;
	var hamburgerClose = $('.hamburger_close');
	var fsOverlay = $('.fs_menu_overlay');

	setHeader();

	$(window).on('resize', function()
	{
		initFixProductBorder();
		setHeader();
	});

	$(document).on('scroll', function()
	{
		setHeader();
	});

	initMenu();
	initFavorite();
	initFixProductBorder();
	initIsotopeFiltering();
	initPriceSlider();
	initCheckboxes();

	/* 

	2. Set Header

	*/

	function setHeader()
	{
		if(window.innerWidth < 992)
		{
			if($(window).scrollTop() > 100)
			{
				header.css({'top':"0"});
			}
			else
			{
				header.css({'top':"0"});
			}
		}
		else
		{
			if($(window).scrollTop() > 100)
			{
				header.css({'top':"-50px"});
			}
			else
			{
				header.css({'top':"0"});
			}
		}
		if(window.innerWidth > 991 && menuActive)
		{
			closeMenu();
		}
	}

	/* 

	3. Init Menu

	*/

	function initMenu()
	{
		if(hamburger.length)
		{
			hamburger.on('click', function()
			{
				if(!menuActive)
				{
					openMenu();
				}
			});
		}

		if(fsOverlay.length)
		{
			fsOverlay.on('click', function()
			{
				if(menuActive)
				{
					closeMenu();
				}
			});
		}

		if(hamburgerClose.length)
		{
			hamburgerClose.on('click', function()
			{
				if(menuActive)
				{
					closeMenu();
				}
			});
		}

		if($('.menu_item').length)
		{
			var items = document.getElementsByClassName('menu_item');
			var i;

			for(i = 0; i < items.length; i++)
			{
				if(items[i].classList.contains("has-children"))
				{
					items[i].onclick = function()
					{
						this.classList.toggle("active");
						var panel = this.children[1];
					    if(panel.style.maxHeight)
					    {
					    	panel.style.maxHeight = null;
					    }
					    else
					    {
					    	panel.style.maxHeight = panel.scrollHeight + "px";
					    }
					}
				}	
			}
		}
	}

	function openMenu()
	{
		menu.addClass('active');
		// menu.css('right', "0");
		fsOverlay.css('pointer-events', "auto");
		menuActive = true;
	}

	function closeMenu()
	{
		menu.removeClass('active');
		fsOverlay.css('pointer-events', "none");
		menuActive = false;
	}

	/* 

	4. Init Favorite

	*/

    function initFavorite()
    {
    	if($('.favorite').length)
    	{
    		var favs = $('.favorite');

    		favs.each(function()
    		{
    			var fav = $(this);
    			var active = false;
    			if(fav.hasClass('active'))
    			{
    				active = true;
    			}

    			fav.on('click', function()
    			{
    				if(active)
    				{
    					fav.removeClass('active');
    					active = false;
    				}
    				else
    				{
    					fav.addClass('active');
    					active = true;
    				}
    			});
    		});
    	}
    }

    /* 

	5. Init Fix Product Border

	*/

    function initFixProductBorder()
    {
    	if($('.product_filter').length)
    	{
			var products = $('.product_filter:visible');
    		var wdth = window.innerWidth;

    		// reset border
    		products.each(function()
    		{
    			$(this).css('border-right', 'solid 1px #e9e9e9');
    		});

    		// if window width is 991px or less

    		if(wdth < 480)
			{
				for(var i = 0; i < products.length; i++)
				{
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}

    		else if(wdth < 576)
			{
				if(products.length < 5)
				{
					var product = $(products[products.length - 1]);
					product.css('border-right', 'none');
				}
				for(var i = 1; i < products.length; i+=2)
				{
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}

    		else if(wdth < 768)
			{
				if(products.length < 5)
				{
					var product = $(products[products.length - 1]);
					product.css('border-right', 'none');
				}
				for(var i = 2; i < products.length; i+=3)
				{
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}

    		else if(wdth < 992)
			{
				if(products.length < 5)
				{
					var product = $(products[products.length - 1]);
					product.css('border-right', 'none');
				}
				for(var i = 2; i < products.length; i+=3)
				{
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}

			//if window width is larger than 991px
			else
			{
				if(products.length < 5)
				{
					var product = $(products[products.length - 1]);
					product.css('border-right', 'none');
				}
				for(var i = 3; i < products.length; i+=4)
				{
					var product = $(products[i]);
					product.css('border-right', 'none');
				}
			}	
    	}
    }

    /* 

	6. Init Isotope Filtering

	*/

	// Hàm định dạng tiền VND (thêm dấu chấm phân cách)
	function formatVND(number) {
		return number.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ".") + "VND";
	}

	function initIsotopeFiltering() {
		var sortTypes = $('.type_sorting_btn');
		var sortNums = $('.num_sorting_btn');
		var sortTypesSelected = $('.sorting_type .item_sorting_btn is-checked span');
		var filterButton = $('.filter_button');

		if ($('.product-grid').length) {
			// Khởi tạo slider khoảng giá (nếu có)
			if ($("#slider-range").length) {
				$("#slider-range").slider({
					range: true,
					min: 0,
					max: 10000000, // 10 triệu
					values: [0, 10000000],
					slide: function (event, ui) {
						// Cập nhật giá trị hiển thị
						$("#amount").val(
							formatVND(ui.values[0]) + " - " + formatVND(ui.values[1])
						);
					}
				});

				// Set giá trị ban đầu cho input
				$("#amount").val(
					formatVND($("#slider-range").slider("values", 0)) + " - " +
					formatVND($("#slider-range").slider("values", 1))
				);
			}

			// Khởi tạo Isotope
			$('.product-grid').isotope({
				itemSelector: '.product-item',
				getSortData: {
					price: function (itemElement) {
						// Xử lý giá VND (xóa dấu chấm và chữ VND)
						var priceEle = $(itemElement).find('.product_price').text()
							.replace('VND', '')
							.replace(/\./g, ''); // Xóa dấu phân cách
						return parseFloat(priceEle);
					},
					name: '.product_name'
				},
				animationOptions: {
					duration: 750,
					easing: 'linear',
					queue: false
				}
			});

			// Xử lý sắp xếp
			sortTypes.each(function () {
				$(this).on('click', function () {
					$('.type_sorting_text').text($(this).text());
					var option = $(this).attr('data-isotope-option');
					option = JSON.parse(option);
					$('.product-grid').isotope(option);
				});
			});

			// Xử lý số lượng sản phẩm hiển thị
			sortNums.each(function () {
				$(this).on('click', function () {
					var numSortingText = $(this).text();
					var numFilter = ':nth-child(-n+' + numSortingText + ')';
					$('.num_sorting_text').text($(this).text());
					$('.product-grid').isotope({ filter: numFilter });
				});
			});

			// Xử lý lọc theo khoảng giá
			filterButton.on('click', function () {
				$('.product-grid').isotope({
					filter: function () {
						var priceRange = $('#amount').val();

						// Tách giá min/max từ chuỗi VND
						var priceParts = priceRange.split(' - ');

						// Xử lý giá min
						var priceMin = parseFloat(
							priceParts[0]
								.replace('VND', '')
								.replace(/\./g, '') // Xóa dấu chấm
								.trim()
						);

						// Xử lý giá max
						var priceMax = parseFloat(
							priceParts[1]
								.replace('VND', '')
								.replace(/\./g, '') // Xóa dấu chấm
								.trim()
						);

						// Lấy giá sản phẩm
						var itemPrice = parseFloat(
							$(this).find('.product_price')
								.clone()
								.children()
								.remove()
								.end()
								.text()
								.replace('VND', '')
								.replace(/\./g, '') // Xóa dấu chấm
						);

						// So sánh giá
						return (itemPrice >= priceMin) && (itemPrice <= priceMax);
					},
					animationOptions: {
						duration: 750,
						easing: 'linear',
						queue: false
					}
				});
			});
		}
	}








    /* 

	7. Init Price Slider

	*/

	function initPriceSlider() {
		var minPrice = 0;
		var maxPrice = 10000000; // 10 triệu VND

		$("#slider-range").slider({
			range: true,
			min: minPrice,
			max: maxPrice,
			values: [minPrice, maxPrice],
			slide: function (event, ui) {
				$("#amount").val(formatVND(ui.values[0]) + " - " + formatVND(ui.values[1]));
			}
		});

		// Set giá trị hiển thị ban đầu
		$("#amount").val(
			formatVND($("#slider-range").slider("values", 0)) + " - " +
			formatVND($("#slider-range").slider("values", 1))
		);
	}


    /* 

	8. Init Checkboxes

	*/

    function initCheckboxes()
    {
    	if($('.checkboxes li').length)
    	{
    		var boxes = $('.checkboxes li');

    		boxes.each(function()
    		{
    			var box = $(this);

    			box.on('click', function()
    			{
    				if(box.hasClass('active'))
    				{
    					box.find('i').removeClass('fa-square');
    					box.find('i').addClass('fa-square-o');
    					box.toggleClass('active');
    				}
    				else
    				{
    					box.find('i').removeClass('fa-square-o');
    					box.find('i').addClass('fa-square');
    					box.toggleClass('active');
    				}
    				// box.toggleClass('active');
    			});
    		});

    		if($('.show_more').length)
    		{
    			var checkboxes = $('.checkboxes');

    			$('.show_more').on('click', function()
    			{
    				checkboxes.toggleClass('active');
    			});
    		}
    	};
    }
});