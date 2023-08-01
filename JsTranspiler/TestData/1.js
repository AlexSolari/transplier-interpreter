function test() {
    return 10;
};

function summ(a, b) {
    return a + b;
}

var foo = 10;
var bar = test() + 15;
var baz = foo + bar;
var zap = summ(bar, baz);

console.log(50);
console.log(foo);
console.log(bar);
console.log(baz);
console.log(zap);