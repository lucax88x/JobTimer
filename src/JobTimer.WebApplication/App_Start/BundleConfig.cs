using System.Diagnostics;
using System.Web.Optimization;
using Autofac;
using JobTimer.WebApplication.Loaders.Master;

namespace JobTimer.WebApplication
{
    public static class BundleConfig
    {
        public static void Register(IContainer container)
        {
            RegisterScripts(container);
            RegisterStylesheets();
        }

        private static void RegisterScripts(IContainer container)
        {
            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/libs")

                .Include("~/bower_components/jquery/dist/jquery.js")
                .Include("~/bower_components/blockUI/jquery.blockUI.js")
                .Include("~/bower_components/nprogress/nprogress.js")
                .Include("~/content/scripts/libs/bootstrap/bootstrap.js")
                .Include("~/bower_components/underscore/underscore.js")
                .Include("~/bower_components/amplify/lib/amplify.store.js")
                .Include("~/bower_components/rsvp.js/rsvp.js")
                .Include("~/content/scripts/libs/basket/basket.js")
                .Include("~/bower_components/angular/angular.js")
                .Include("~/bower_components/angular-animate/angular-animate.js")
                .Include("~/bower_components/angular-ui-router/release/angular-ui-router.js")
                .Include("~/content/scripts/libs/jquery/jquery-multilevelpushmenu.js")
                .Include("~/bower_components/scrollup/dist/jquery.scrollup.js")
                .Include("~/content/scripts/libs/bootstrap/bootstrap-navbarhiding.js")
                .Include("~/content/scripts/libs/bootstrap/bootstrap-select.js")
                .Include("~/bower_components/bootstrap-datepicker/dist/js/bootstrap-datepicker.js")
                .Include("~/content/scripts/libs/bootstrap/typeahead.js")
                .Include("~/bower_components/handlebars/handlebars.js")
                .Include("~/bower_components/moment/moment.js")
                .Include("~/bower_components/moment/locale/en.js")
                .Include("~/bower_components/jstimezonedetect/jstz.js")                
                .Include("~/bower_components/toastr/toastr.js")

                .Include("~/scripts/jquery.signalR-2.2.0.js")
                );


            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/login")

                .Include("~/content/scripts/libs/codrops/tabs.js")

                .Include("~/app/login/app.js")
                .Include("~/app/login/controllers/login.js")
                );

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/core")
                .Include("~/app/config.js")
                .Include("~/app/generated/constants.js")
                .Include("~/app/generated/enums.js")

                .Include("~/app/core/core.js")
                .Include("~/app/core/providers/initializer.js")
                .Include("~/app/core/factories/ajaxer.js")                
                .Include("~/app/core/factories/locker.js")
                .Include("~/app/core/factories/cache.js")
                .Include("~/app/core/factories/hubs.js")
                .Include("~/app/core/factories/notifier.js")
                .Include("~/app/core/factories/store.js")
                .Include("~/app/core/directives/modal-visible.js")
                .Include("~/app/core/directives/locker.js")
                .Include("~/app/core/directives/nav.js")
                .Include("~/app/core/directives/hidden.js")
                .Include("~/app/core/directives/multi-menu.js")
                .Include("~/app/core/directives/multi-menu-button.js")
                .Include("~/app/core/directives/scroll-up.js")
                .Include("~/app/core/directives/select.js")
                .Include("~/app/core/directives/datepicker.js")
                .Include("~/app/core/directives/search.js")
                .Include("~/app/core/directives/boolean-icon.js")
                .Include("~/app/core/directives/focus.js")
                .Include("~/app/core/directives/dropdown.js")
                .Include("~/app/core/directives/tabs.js")
                );

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/app")
                .Include("~/app/app/app.js")
                .Include("~/app/app/controllers/master.js")
                .Include("~/app/app/controllers/estimation.js")
                .Include("~/app/app/controllers/user-data.js")
                .Include("~/app/app/factories/user-data.js")
                );

            BundleTable.Bundles.Add(new ScriptBundle("~/bundles/help")
                .Include("~/bower_components/intro.js/minified/intro.min.js")
                .Include("~/app/app/controllers/help.js")
                );
            
            var bundlesLoader = container.Resolve<IBundlesLoader>();
            var bundles = bundlesLoader.Load();

            if (bundles != null)
            {
                foreach (var bundle in bundles.Bundles)
                {
                    BundleTable.Bundles.Add(new ScriptBundle(string.Format("~/bundles/{0}", bundle.Bundle))
                        .Include(bundle.Scripts.ToArray()));
                }
            }
        }
        private static void RegisterStylesheets()
        {
            BundleTable.Bundles.Add(new StyleBundle("~/css/common")
                .Include("~/content/css/bootstrap/bootstrap.css")
                .Include("~/content/css/bootstrap/bootstrap-theme.css")
                .Include("~/content/css/bootstrap/template.css")
                .Include("~/bower_components/font-awesome/css/font-awesome.min.css", new CssRewriteUrlTransform())                
                .Include("~/content/css/glyphicons/glyphicons.css")
                .Include("~/bower_components/nprogress/nprogress.css")
                .Include("~/content/css/nprogress/nprogress.css")
                .Include("~/bower_components/toastr/toastr.css")
                .Include("~/content/css/toastr/toastr.css")
                .Include("~/content/css/custom/common.css")
                );

            BundleTable.Bundles.Add(new StyleBundle("~/css/login")
                .Include("~/content/css/codrops/tabs.css")
                .Include("~/content/css/codrops/tabsstyles.css")
                .Include("~/content/css/custom/login.css", new CssRewriteUrlTransform())
                );

            BundleTable.Bundles.Add(new StyleBundle("~/css/core")
                .Include("~/content/css/jquery/jquery.multilevelpushmenu.css")
                .Include("~/content/css/jquery/jquery.scrollup.css")
                .Include("~/bower_components/bootstrap-datepicker/dist/css/bootstrap-datepicker.css")
                .Include("~/content/css/bootstrap/typeahead.css")
                .Include("~/content/css/bootstrap/bootstrap-select.css")
                .Include("~/content/css/bootstrap/bootstrap-select-ajax.css")
                .Include("~/content/css/custom/master.css", new CssRewriteUrlTransform())
                );

            BundleTable.Bundles.Add(new StyleBundle("~/css/help")
                .Include("~/bower_components/intro.js/minified/introjs.min.css")
                .Include("~/bower_components/intro.js/themes/introjs-nassim.css")
                .Include("~/content/css/custom/help.css")
                );

            BundleTable.Bundles.Add(new StyleBundle("~/css/home")
                .Include("~/content/css/bootstrap/bootstrap-tiles.css")
                );
            
            BundleTable.Bundles.Add(new StyleBundle("~/css/timer")
                .Include("~/content/css/progress-button/progress-button.css")
                .Include("~/bower_components/seiyria-bootstrap-slider/dist/css/bootstrap-slider.css")
                .Include("~/content/css/slider/slider.css")
                .Include("~/bower_components/angular-circular-slider/dist/css/angular-circular-slider.min.css")                
                );

            BundleTable.Bundles.Add(new StyleBundle("~/css/grid")
                .Include("~/content/css/slickgrid/slick.grid.css")
                .Include("~/content/css/slickgrid/slick.grid.customization.css")
                .Include("~/content/css/bootstrap/bootstrap-switch.css")
                );
        }
    }
}