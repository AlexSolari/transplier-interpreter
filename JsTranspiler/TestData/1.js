var bar = 15;
var foo = bar < 10;

if (foo) {
    console.log(true);
} else if (false) {
    console.log("unreachable")
} else {
    console.log("nested")
}