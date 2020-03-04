<html>
    <head>
        <meta http-equiv="X-UA-Compatible" content="IE=edge" />
    </head>
    <style>
        html{
            overflow:hidden;
        }
        body{
            width: auto;
            height: auto;
            background-color: #373737;
            font-family: sans-serif;
        }
        .notes{
            color: #269e5b;
            font-size: 15px;
            font-weight: bold;
            padding: 3px;
        }
        .post{
            background-color: #2d2d2d;
            position: relative;
            padding-bottom: 10px;
        }
        .version{
            color: #269e5b;
            font-size: 13px;
            font-weight: bold;
            padding: 5px;
        }
        .desc{
            color: #8d8d8d;
            padding: 5px;
            font-size: 15px;
        }
        .desc ul{
            margin: 0px 0px 10px 0px;
            padding-left: 30px;
        }
        .date{
            position: absolute;
            bottom: 0px;
            right: 0px;
            color: #8d8d8d;
            padding: 3px;
            font-size: 12px;
        }
    </style>
    <body>
        <div class="notes">Update Notes:</div>
        <?php

        $servername = "127.0.0.1";
        $username = "root";
        $password = "";
        $dbname = "...";

        // creat connect
        $conn = new mysqli($servername, $username, $password, $dbname);
        // check connect

        if ($conn->connect_error) {
            die("Error in database connect.");
        }

        $versions=$conn->query("SELECT title,post_date,message FROM xf_version_update WHERE resource_update_id = 199 ORDER BY resource_update_id DESC LIMIT 1");

            if($versions->num_rows > 0){

                while($data = $versions->fetch_assoc()) {
                    echo "<div class='post'>";
                    echo "<div class='version'>".$data['title']."</div>";
                    echo "<div class='desc'>".$data['message']."</div>";
                    echo "<div class='date'>".date("d/m/Y H:i A", strtotime($data['post_date']))."</div>";
                    echo "</div>";
                }

            }

        ?>
    </body>
</html>
