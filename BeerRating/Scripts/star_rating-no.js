/*!
 * Star Rating Norwegian Translations
 *
 * This file must be loaded after 'star-rating.js'. Patterns in braces '{}', or
 * any HTML markup tags in the messages must not be converted or translated.
 *
 * NOTE: this file must be saved in UTF-8 encoding.
 *
 * @see http://github.com/kartik-v/bootstrap-star-rating
 * @author Kartik Visweswaran <kartikv2@gmail.com>
 */
(function ($) {
    "use strict";
    $.fn.ratingLocales['no'] = {
        defaultCaption: '{rating} Stjerne',
        starCaptions: {
            0.5: 'Halv stjerne',
            1: 'En stjerne',
            1.5: 'En og en halv stjerne',
            2: 'To stjerner',
            2.5: 'To og en halv stjerne',
            3: 'Tre stjerner',
            3.5: 'Tre og en halv stjerne',
            4: 'Fire stjerner',
            4.5: 'Fire og en halv stjerne',
            5: 'Fem stjerner',
            5.5: 'Fem og en halv stjerne',
            6: 'Seks stjerner'
        },
        clearButtonTitle: 'Restart',
        clearCaption: 'Ikke vurdert'
    };
})(window.jQuery);
