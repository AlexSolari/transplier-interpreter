var foo = 10;

switch (foo) {
    case 10:
        console.log(foo);
        break;
    case 20:
    case 30:
        console.log(false);
        throw new Exception();
        break;
    default:
        break;
}