if (typeof (fodt) == "undefined") fodt = {};

fodt.SlideshowController = function (container) {
    var controller = this;
    controller.container = container;
    controller.images = container.find("li");
}
$.extend(fodt.SlideshowController.prototype, {
    slideshowIntervalInMilliseconds: 5000,
    intervalHandle: null,
    currentIndex: -1,
    start: function () {
        var controller = this;
        this.images.css("display", "none").css("position", "absolute").css("opacity", "0");
        this.images.on("click", function () { controller.resetInterval(); controller.showNextImage(); });
        this.resetInterval();
        this.showNextImage();
    },
    resetInterval: function () {
        var controller = this;
        if (controller.intervalHandle != null) {
            clearInterval(controller.intervalHandle);
            controller.intervalHandle = null;
        }
        this.intervalHandle = setInterval(function () { controller.onInterval() }, controller.slideshowIntervalInMilliseconds);
    },
    onInterval: function () {
        this.showNextImage();
    },
    showImage: function (index) {
        if (index >= this.images.length) return;
        if (this.currentIndex != -1) {
            var previousImage = $(this.images[this.currentIndex]);
            previousImage.animate({ opacity: 0 }, 500, function () { previousImage.css("display", "none"); });
        }
        this.currentIndex = index;
        var currentImage = $(this.images[this.currentIndex]);
        currentImage.css("display", "block").animate({ opacity: 1 }, 500);
    },
    showNextImage: function () {
        var nextIndex = this.currentIndex + 1;
        if (nextIndex >= this.images.length) {
            nextIndex = 0;
        }
        this.showImage(nextIndex);
    }
});