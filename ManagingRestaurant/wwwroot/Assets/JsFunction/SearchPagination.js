let searchInput = document.getElementById('searchInput');
let valueInput = searchInput.value;

if(searchInput != null) {
    let pagination = document.querySelector('.pagination');
    pagination.querySelectorAll('a').forEach(a => {
        a.href = a.href + `&searchString=${valueInput}`;
    })
}
