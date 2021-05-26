import { Component } from "react";
import Field from "../field/field";
import styles from "./game.module.css";
import { range } from "lodash";
import Ability, { AbilityEnum } from "../ability/ability";
import Controller from "../../core/controller";

export interface GameProps {
    controller: Controller;
}

export default class Game extends Component<GameProps> {
    linesCount = 11;
    private shoot(index: number) {
        this.props.controller.shoot(index);
    }

    private getShootHotkey(index: number): string {
        if (index === 10){ 
            return "0";
        }
        if (index === 11){ 
            return "-";
        }
        return String(index);
    }

    render() {
        return <div className={styles.game}>
            <div className={styles.fieldContainer}>
                <Field fieldState={this.props.controller.getFieldState()}/>
                <div className={styles.arrowContainer}>
                    {range(1, this.linesCount + 1, 1).map(arrowIndex => <Ability hotkey={this.getShootHotkey(arrowIndex)} onClick={() => this.shoot(arrowIndex)} ability={AbilityEnum.Shoot} />)}
                </div>
            </div>
            <div className={styles.actionsContainer}>
                <div className={styles.resources}>
                    <div className={styles.health}>Health</div>
                    <div className={styles.energy}>Energy</div>
                </div>
                <div className={styles.abilities}>Abilities</div>
                <div className={styles.items}>Items</div>
                <div className={styles.info}>Info (Level, time left, score, gold)</div>
                <div className={styles.log}>Log</div>
            </div>
        </div>
    }
}