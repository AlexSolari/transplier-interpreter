var foo = 15;

if (10 < 20)
    return false;

if (10 + 20 > 15){
    return true;
}

if (foo){
    return false;
} else {
    return 0;
}

if (foo > 20)
    return 0;
else if (foo < 10){
    return "res";
}
else {
    console.log(foo);
}