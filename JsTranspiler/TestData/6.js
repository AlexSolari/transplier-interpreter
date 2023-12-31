var Command = require("../entities/command");

class CommandBuilder{
    constructor(name)
    {
        this.name = name;
        this.trigger = null;
        this.active = true;
        this.condition = () => true;
        this.handler = () => {};
    }

    on(trigger){
        this.trigger = trigger;

        return this;
    }

    when(condition){
        this.condition = condition;

        return this;
    }

    do(handler){
        this.handler = handler;

        return this;
    }

    disabled(){
        this.active = false;

        return this;
    }

    build(){
        return new Command(this.trigger, this.condition, this.handler, this.name, this.active);
    }
}

module.exports = CommandBuilder;