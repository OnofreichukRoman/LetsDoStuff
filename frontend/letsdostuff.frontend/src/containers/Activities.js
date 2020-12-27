import React, { useState } from "react";
import Button from "react-bootstrap/Button";
import "./Activities.css";
import axios from "axios";
import Jumbotron from 'react-bootstrap/Jumbotron'
import "./CommonStyles.css";
import {ActivityCard} from "./ActivityCard";
import { Link } from "react-router-dom";

function Activities() {
  const [activities, setActivities] = useState([]);

  function filterActivities() {
    if (activities.length === 0) {
      axios.get("https://localhost:8081/api/activities")
      .then((resp) => {
        setActivities(resp.data);
      })
      .catch((err) => console.log(err));
    }
  }

  filterActivities();
  return (
    <div className="Activities">
      <Jumbotron className="JumbotronStyle">
      <>
                <Link to={"/activities/create"}>
                  <Button variant="primary">Create activity</Button>
                </Link>
              </>
        {activities.map((activity) => (
          <>
            {ActivityCard({activity}, ()=>(
              <>
                <Link to={"/activities/" + activity.id}>
                  <Button variant="outline-info">Details</Button>
                </Link>
              </>
            ))}
          </>
        ))}
      </Jumbotron>
    </div>
  );
}

export default Activities;
