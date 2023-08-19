import React, { Component } from 'react';

class StarRating extends Component {
    constructor(props) {
        super(props)
        this.state = {
            rating: 0,
            hover: 0
        }
    }

    componentWillReceiveProps(props) {
        this.setState({ rating: props.starRating })
    }

    setRating(index) {
        this.setState({
            rating: index
        })
    }

    setHover(index) {
        this.setState({
            hover: index
        })
    }

    render() {
        return (
            <div className="star-rating">
                {[...Array(5)].map((star, index) => {
                    index += 1;
                    return (
                        <button
                            className={index <= (this.state.hover || this.state.rating) ? "on" : "off"}
                            type="button"
                            key={index}
                            disabled={true}
                            //onClick={this.setRating(index)}
                            //onMouseEnter={this.setHover(index)}
                            //onMouseLeave={this.setHover(rating)}
                        >
                            <span className="star">&#9733;</span>
                        </button>
                    );
                })}
            </div>
        );
    }
}

export default StarRating;
