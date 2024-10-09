document.getElementById('formFileMultiple').addEventListener('change', function() {
    var files = this.files;
    var imageContainer = document.querySelector(".display-image-more");
    imageContainer.innerHTML = ''; // Clear previous images

    for (var i = 0; i < files.length; i++) {
        var file = files[i];
        if (file.type.startsWith('image/')) {
            var reader = new FileReader();
            reader.onload = function(event) {
                let div = document.createElement('div');
                let imgTag = `<img src="${event.target.result}" style="margin: 10px; width: 100px; height: 100px; border: 2px solid #8f8a8a;border-radius: 3px"/>`;
                div.innerHTML = imgTag;
                imageContainer.appendChild(div);
            };
            reader.readAsDataURL(file);
        }
    }
});